using System.Threading.Channels;
using SmartRecruiter.Application.DTOs;

namespace SmartRecruiter.Application.Services;

public class CandidateQueueService
{
    private readonly Channel<CreateCandidateRequest> _queue;

    public CandidateQueueService()
    {
        var options = new BoundedChannelOptions(100) // fix we need to make limited queue because server can break if there is too much emails
        {
            SingleReader = true,
            SingleWriter = false, 
            
            FullMode = BoundedChannelFullMode.Wait 
        };

        _queue = Channel.CreateBounded<CreateCandidateRequest>(options);
    }

    public async ValueTask EnqueueAsync(CreateCandidateRequest request, CancellationToken ct = default)
    {
        await _queue.Writer.WriteAsync(request, ct);
    }

    public IAsyncEnumerable<CreateCandidateRequest> ReadAllAsync(CancellationToken ct)
    {
        return _queue.Reader.ReadAllAsync(ct);
    }
}