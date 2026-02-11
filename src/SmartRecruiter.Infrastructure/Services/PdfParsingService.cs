using System.Text;
using SmartRecruiter.Domain.Interfaces;
using UglyToad.PdfPig;

namespace SmartRecruiter.Infrastructure.Services;

public class PdfParsingService: IFileParsingService
{
    public async Task<string> ExtractTextAsync(Stream stream)
    {
        if (stream.CanSeek && stream.Position > 0)
        {
            stream.Position = 0;
        }
        
        return await Task.Run(() =>
        {
            var sb = new StringBuilder();
            
            using (var document = PdfDocument.Open(stream))
            {
                foreach (var page in document.GetPages())
                {
                    sb.Append(page.Text);
                    sb.Append(" ");
                }
            }
            
            return sb.ToString().Trim();
        });
        
    }
}