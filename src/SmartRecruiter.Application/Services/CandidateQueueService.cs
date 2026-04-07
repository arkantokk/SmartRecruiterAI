using System.Threading.Channels;
using SmartRecruiter.Application.DTOs;

namespace SmartRecruiter.Application.Services;

public class CandidateQueueService
{
    private readonly Channel<CreateCandidateRequest> _queue;

    public CandidateQueueService()
    {
        var options = new UnboundedChannelOptions { SingleReader = true };
        _queue = Channel.CreateUnbounded<CreateCandidateRequest>(options);
    }

    public async ValueTask EnqueueAsync(CreateCandidateRequest request)
    {
        await _queue.Writer.WriteAsync(request);
    }

    public IAsyncEnumerable<CreateCandidateRequest> ReadAllAsync(CancellationToken ct)
    {
        return _queue.Reader.ReadAllAsync(ct);
    }
}