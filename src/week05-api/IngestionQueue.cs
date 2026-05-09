using System.Threading.Channels;

namespace Week05;

public interface IIngestionQueue
{
    ValueTask QueueBackgroundWorkItemAsync(string filePath);
    ValueTask<string> DequeueAsync(CancellationToken cancellationToken);
}

public class IngestionQueue : IIngestionQueue
{
    private readonly Channel<string> _queue;

    public IngestionQueue()
    {
        // Simple unbounded channel for now
        _queue = Channel.CreateUnbounded<string>();
    }

    public async ValueTask QueueBackgroundWorkItemAsync(string filePath)
    {
        await _queue.Writer.WriteAsync(filePath);
    }

    public async ValueTask<string> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}
