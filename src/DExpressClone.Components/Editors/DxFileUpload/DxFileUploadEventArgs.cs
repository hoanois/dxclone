namespace DExpressClone.Components.Editors;

/// <summary>
/// Provides data for the DxFileUpload upload event.
/// </summary>
public class DxFileUploadEventArgs
{
    /// <summary>
    /// Gets the file being uploaded.
    /// </summary>
    public DxFileUploadFile File { get; init; } = default!;

    /// <summary>
    /// Gets the stream to read the file content from.
    /// </summary>
    public Stream FileStream { get; init; } = default!;

    /// <summary>
    /// Gets the progress reporter to update upload progress (0-100).
    /// </summary>
    public IProgress<int> Progress { get; init; } = default!;

    /// <summary>
    /// Gets the cancellation token to observe for upload cancellation.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }
}
