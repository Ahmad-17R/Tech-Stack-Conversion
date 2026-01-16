using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VetSuccess.Application.Interfaces;
using VetSuccess.Infrastructure.Configuration;

namespace VetSuccess.Infrastructure.Services;

public class AzureBlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureBlobStorageService> _logger;
    private readonly AzureStorageOptions _options;

    public AzureBlobStorageService(
        IOptions<AzureStorageOptions> options,
        ILogger<AzureBlobStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;
        _blobServiceClient = new BlobServiceClient(_options.ConnectionString);
    }

    public async Task<string> UploadFileAsync(
        string containerName,
        string fileName,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            if (_options.CreateContainerIfNotExists)
            {
                await containerClient.CreateIfNotExistsAsync(
                    PublicAccessType.None,
                    cancellationToken: cancellationToken);
            }

            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(content, overwrite: true, cancellationToken);

            _logger.LogInformation("File uploaded successfully: {Container}/{FileName}", containerName, fileName);
            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {Container}/{FileName}", containerName, fileName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileStreamAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var response = await blobClient.DownloadAsync(cancellationToken);
            _logger.LogInformation("File downloaded successfully: {Container}/{FileName}", containerName, fileName);

            return response.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {Container}/{FileName}", containerName, fileName);
            throw;
        }
    }

    public async Task<byte[]> DownloadFileBytesAsync(
        string containerName,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(filePath);

            using var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream, cancellationToken);
            
            _logger.LogInformation("File downloaded successfully as bytes: {Container}/{FilePath}", containerName, filePath);
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file as bytes: {Container}/{FilePath}", containerName, filePath);
            throw;
        }
    }

    public async Task<List<string>> ListFilesAsync(
        string containerName,
        string prefix,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var files = new List<string>();

            await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
            {
                files.Add(blobItem.Name);
            }

            _logger.LogInformation("Listed {Count} files in {Container} with prefix {Prefix}", 
                files.Count, containerName, prefix);
            
            return files;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing files: {Container}/{Prefix}", containerName, prefix);
            throw;
        }
    }

    public async Task DeleteFileAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("File deleted successfully: {Container}/{FileName}", containerName, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {Container}/{FileName}", containerName, fileName);
            throw;
        }
    }

    public async Task<bool> FileExistsAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            return await blobClient.ExistsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking file existence: {Container}/{FileName}", containerName, fileName);
            return false;
        }
    }
}
