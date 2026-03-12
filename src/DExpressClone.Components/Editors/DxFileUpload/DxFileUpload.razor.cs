using DExpressClone.Components.Core;
using DExpressClone.Components.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace DExpressClone.Components.Editors.DxFileUpload;

/// <summary>
/// A file upload component with drag-and-drop support, progress tracking, and image preview.
/// </summary>
public partial class DxFileUpload : DxInteractiveComponentBase, IAsyncDisposable
{
    [Inject]
    private JsInteropService JsInterop { get; set; } = default!;

    private ElementReference _dropZoneRef;
    private bool _jsInitialized;

    /// <summary>
    /// Gets or sets the accepted file types (e.g., ".jpg,.png", "image/*").
    /// </summary>
    [Parameter]
    public string? Accept { get; set; }

    /// <summary>
    /// Gets or sets whether multiple files can be selected. Default is true.
    /// </summary>
    [Parameter]
    public bool Multiple { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum allowed file size in bytes. Default is 10 MB.
    /// </summary>
    [Parameter]
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024;

    /// <summary>
    /// Gets or sets the maximum number of files allowed. Default is 10.
    /// </summary>
    [Parameter]
    public int MaxFileCount { get; set; } = 10;

    /// <summary>
    /// Gets or sets the text displayed in the drop zone. Default is "Drag and Drop File Here".
    /// </summary>
    [Parameter]
    public string UploadText { get; set; } = "Drag and Drop File Here";

    /// <summary>
    /// Gets or sets whether image previews are shown. Default is true.
    /// </summary>
    [Parameter]
    public bool AllowImagePreview { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum file size for generating image previews. Default is 5 MB.
    /// </summary>
    [Parameter]
    public long MaxPreviewFileSize { get; set; } = 5 * 1024 * 1024;

    /// <summary>
    /// Gets or sets whether files are uploaded automatically when selected. Default is false.
    /// </summary>
    [Parameter]
    public bool AutoUpload { get; set; } = false;

    /// <summary>
    /// Gets or sets the upload handler. Called for each file to be uploaded.
    /// </summary>
    [Parameter]
    public Func<DxFileUploadEventArgs, Task>? Upload { get; set; }

    /// <summary>
    /// Callback invoked when a file is added to the selection.
    /// </summary>
    [Parameter]
    public EventCallback<DxFileUploadFile> FileAdded { get; set; }

    /// <summary>
    /// Callback invoked when a file is removed from the selection.
    /// </summary>
    [Parameter]
    public EventCallback<DxFileUploadFile> FileRemoved { get; set; }

    /// <summary>
    /// Callback invoked when the file list changes.
    /// </summary>
    [Parameter]
    public EventCallback<IReadOnlyList<DxFileUploadFile>> FilesChanged { get; set; }

    /// <summary>
    /// Custom template for the drop zone area.
    /// </summary>
    [Parameter]
    public RenderFragment? DropZoneTemplate { get; set; }

    private readonly List<DxFileUploadFile> _files = new();
    private string _inputId = string.Empty;

    private string RootCssClass => CssClassBuilder.New("dx-fileupload")
        .Add(InteractiveStateCssClass)
        .Add(CssClass)
        .Build();

    private string DropZoneCssClass => CssClassBuilder.New("dx-fileupload-dropzone")
        .AddIf("dx-state-disabled", !Enabled)
        .Build();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _inputId = $"dx-fileupload-input-{Id ?? Guid.NewGuid().ToString("N")[..8]}";
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsInitialized = true;
            await JsInterop.SetupDropZoneAsync(_dropZoneRef, _inputId);
        }
    }

    private async Task HandleFilesSelected(InputFileChangeEventArgs e)
    {
        if (!Enabled || ReadOnly) return;

        var files = Multiple ? e.GetMultipleFiles(MaxFileCount) : new[] { e.File };

        foreach (var browserFile in files)
        {
            var validationError = ValidateFile(browserFile);
            if (validationError is not null)
            {
                var errorFile = new DxFileUploadFile
                {
                    Name = browserFile.Name,
                    Size = browserFile.Size,
                    ContentType = browserFile.ContentType,
                    BrowserFile = browserFile,
                    Status = FileUploadStatus.Error,
                    ErrorMessage = validationError
                };
                _files.Add(errorFile);
                await FileAdded.InvokeAsync(errorFile);
                continue;
            }

            var uploadFile = new DxFileUploadFile
            {
                Name = browserFile.Name,
                Size = browserFile.Size,
                ContentType = browserFile.ContentType,
                BrowserFile = browserFile,
                Status = FileUploadStatus.Pending
            };

            _files.Add(uploadFile);
            await FileAdded.InvokeAsync(uploadFile);

            if (AllowImagePreview && IsImageFile(browserFile.ContentType) && browserFile.Size <= MaxPreviewFileSize)
            {
                await LoadPreviewAsync(uploadFile);
            }
        }

        await FilesChanged.InvokeAsync(_files.AsReadOnly());
        RequestRender();

        if (AutoUpload && Upload is not null)
        {
            await UploadAllAsync();
        }
    }

    /// <summary>
    /// Triggers the file browser dialog programmatically.
    /// </summary>
    public async Task BrowseAsync()
    {
        if (!Enabled || ReadOnly) return;
        await JsInterop.TriggerFileInputAsync(_inputId);
    }

    /// <summary>
    /// Removes a file from the selection.
    /// </summary>
    public async Task RemoveFileAsync(DxFileUploadFile file)
    {
        if (file.Status == FileUploadStatus.Uploading) return;

        _files.Remove(file);
        await FileRemoved.InvokeAsync(file);
        await FilesChanged.InvokeAsync(_files.AsReadOnly());
        RequestRender();
    }

    /// <summary>
    /// Uploads all pending files by invoking the Upload callback for each.
    /// </summary>
    public async Task UploadAllAsync()
    {
        if (Upload is null) return;

        var pendingFiles = _files.Where(f => f.Status == FileUploadStatus.Pending).ToList();
        foreach (var file in pendingFiles)
        {
            await UploadFileAsync(file);
        }
    }

    private async Task UploadFileAsync(DxFileUploadFile file)
    {
        if (Upload is null) return;

        file.Status = FileUploadStatus.Uploading;
        file.ProgressPercent = 0;
        RequestRender();

        try
        {
            using var cts = new CancellationTokenSource();
            var stream = file.BrowserFile.OpenReadStream(MaxFileSize, cts.Token);
            var progress = new Progress<int>(percent =>
            {
                file.ProgressPercent = Math.Clamp(percent, 0, 100);
                RequestRender();
            });

            var args = new DxFileUploadEventArgs
            {
                File = file,
                FileStream = stream,
                Progress = progress,
                CancellationToken = cts.Token
            };

            await Upload(args);

            file.Status = FileUploadStatus.Success;
            file.ProgressPercent = 100;
        }
        catch (Exception ex)
        {
            file.Status = FileUploadStatus.Error;
            file.ErrorMessage = ex.Message;
        }

        RequestRender();
    }

    private string? ValidateFile(IBrowserFile file)
    {
        if (file.Size > MaxFileSize)
        {
            return $"File exceeds maximum size of {FormatFileSize(MaxFileSize)}.";
        }

        if (_files.Count(f => f.Status != FileUploadStatus.Error) >= MaxFileCount)
        {
            return $"Maximum of {MaxFileCount} files allowed.";
        }

        if (!string.IsNullOrEmpty(Accept))
        {
            var accepted = Accept.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var isAccepted = accepted.Any(a =>
            {
                if (a.StartsWith('.'))
                    return file.Name.EndsWith(a, StringComparison.OrdinalIgnoreCase);
                if (a.EndsWith("/*"))
                    return file.ContentType.StartsWith(a[..^1], StringComparison.OrdinalIgnoreCase);
                return string.Equals(file.ContentType, a, StringComparison.OrdinalIgnoreCase);
            });

            if (!isAccepted)
            {
                return $"File type '{file.ContentType}' is not accepted.";
            }
        }

        return null;
    }

    private async Task LoadPreviewAsync(DxFileUploadFile file)
    {
        try
        {
            var format = file.ContentType;
            var resizedFile = await file.BrowserFile.RequestImageFileAsync(format, 200, 200);
            using var stream = resizedFile.OpenReadStream(MaxPreviewFileSize);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());
            file.PreviewDataUrl = $"data:{format};base64,{base64}";
            RequestRender();
        }
        catch
        {
            // Preview generation failed — silently ignore
        }
    }

    private static string GetFileItemCssClass(DxFileUploadFile file) => file.Status switch
    {
        FileUploadStatus.Error => "dx-fileupload-item--error",
        FileUploadStatus.Success => "dx-fileupload-item--success",
        FileUploadStatus.Uploading => "dx-fileupload-item--uploading",
        _ => string.Empty
    };

    private static string FormatFileSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):F1} MB";
        return $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
    }

    private static string GetFileIcon(string contentType)
    {
        if (string.IsNullOrEmpty(contentType)) return "\U0001F4C4"; // document
        if (contentType.StartsWith("image/")) return "\U0001F5BC"; // picture
        if (contentType.StartsWith("video/")) return "\U0001F3AC"; // video
        if (contentType.StartsWith("audio/")) return "\U0001F3B5"; // music
        if (contentType.Contains("pdf")) return "\U0001F4D1"; // PDF
        if (contentType.Contains("zip") || contentType.Contains("compressed") || contentType.Contains("archive"))
            return "\U0001F4E6"; // archive
        if (contentType.Contains("spreadsheet") || contentType.Contains("excel"))
            return "\U0001F4CA"; // chart
        if (contentType.Contains("document") || contentType.Contains("word"))
            return "\U0001F4DD"; // memo
        return "\U0001F4C4"; // document
    }

    private static bool IsImageFile(string contentType)
    {
        return !string.IsNullOrEmpty(contentType) && contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }

    public async ValueTask DisposeAsync()
    {
        if (_jsInitialized)
        {
            try
            {
                await JsInterop.RemoveDropZoneAsync(_dropZoneRef);
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected, ignore
            }
        }
    }
}
