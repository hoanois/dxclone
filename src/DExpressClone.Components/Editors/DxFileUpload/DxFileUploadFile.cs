using Microsoft.AspNetCore.Components.Forms;

namespace DExpressClone.Components.Editors;

/// <summary>
/// Represents a file selected for upload in the DxFileUpload component.
/// </summary>
public class DxFileUploadFile
{
    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the file size in bytes.
    /// </summary>
    public long Size { get; init; }

    /// <summary>
    /// Gets the MIME content type of the file.
    /// </summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the current upload status.
    /// </summary>
    public FileUploadStatus Status { get; set; } = FileUploadStatus.Pending;

    /// <summary>
    /// Gets or sets the upload progress percentage (0-100).
    /// </summary>
    public int ProgressPercent { get; set; }

    /// <summary>
    /// Gets or sets the error message if the upload failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the data URL for image preview.
    /// </summary>
    public string? PreviewDataUrl { get; set; }

    /// <summary>
    /// Gets the underlying Blazor browser file reference.
    /// </summary>
    internal IBrowserFile BrowserFile { get; init; } = default!;
}

/// <summary>
/// Represents the upload status of a file.
/// </summary>
public enum FileUploadStatus
{
    /// <summary>File is selected but not yet uploaded.</summary>
    Pending,
    /// <summary>File is currently being uploaded.</summary>
    Uploading,
    /// <summary>File was uploaded successfully.</summary>
    Success,
    /// <summary>File upload failed.</summary>
    Error
}
