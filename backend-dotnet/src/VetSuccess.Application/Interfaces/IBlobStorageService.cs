namespace VetSuccess.Application.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(string containerName, string fileName, Stream content, CancellationToken cancellationToken = default);
    Task<Stream> DownloadFileStreamAsync(string containerName, string fileName, CancellationToken cancellationToken = default);
    Task<byte[]> DownloadFileBytesAsync(string containerName, string filePath, CancellationToken cancellationToken = default);
    Task<List<string>> ListFilesAsync(string containerName, string prefix, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string containerName, string fileName, CancellationToken cancellationToken = default);
    Task<bool> FileExistsAsync(string containerName, string fileName, CancellationToken cancellationToken = default);
}
