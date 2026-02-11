namespace SmartRecruiter.Domain.Interfaces;

public interface IFileParsingService
{
    Task<string> ExtractTextAsync(Stream stream);
}