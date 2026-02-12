namespace SmartRecruiter.Domain.Interfaces;

public interface IStorageService
{
   Task<string> UploadAsync(Stream stream, string fileName, string contentType);
   string GenerateReadOnlyUrl(string blobUrl, TimeSpan expiry);
}