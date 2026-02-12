using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.Infrastructure.Services;

public class BlobStorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage") 
                               ?? throw new ArgumentNullException("AzureStorage connection string is missing");
        
        _containerName = configuration["AzureStorage:ContainerName"] ?? "resumes";
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var trustedFileName = $"{Guid.NewGuid()}_{fileName}";
        var blobClient = containerClient.GetBlobClient(trustedFileName);

        var blobOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(stream, blobOptions);

        return blobClient.Uri.ToString();
    }

    public string GenerateReadOnlyUrl(string blobUrl, TimeSpan expiry)
    {
        var blobClient = _blobServiceClient
            .GetBlobContainerClient(_containerName)
            .GetBlobClient(Path.GetFileName(blobUrl));
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobClient.Name,
            Resource = "b", // "b" означає Blob
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiry)
        };
        
        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        
        Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

        return sasUri.ToString();
    }
}