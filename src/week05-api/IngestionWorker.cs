using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace Week05;

public class IngestionWorker : BackgroundService
{
    private readonly IIngestionQueue _queue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IngestionWorker> _logger;

    public IngestionWorker(
        IIngestionQueue queue, 
        IServiceProvider serviceProvider, 
        ILogger<IngestionWorker> logger)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Ingestion Worker is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var filePath = await _queue.DequeueAsync(stoppingToken);

            try
            {
                _logger.LogInformation("Processing ingestion for: {FilePath}", filePath);

                using var scope = _serviceProvider.CreateScope();
                var memory = scope.ServiceProvider.GetRequiredService<IKernelMemory>();

                // Ingest the file
                await memory.ImportDocumentAsync(filePath);

                _logger.LogInformation("Successfully ingested: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ingestion for: {FilePath}", filePath);
            }
        }

        _logger.LogInformation("Ingestion Worker is stopping.");
    }
}
