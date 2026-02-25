using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using CqrsMediatRDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CqrsMediatRDemo.Infrastructure.Services;

public class OutboxProcessorBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorBackgroundService> _logger;

    public OutboxProcessorBackgroundService(IServiceProvider serviceProvider, ILogger<OutboxProcessorBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>(); // Assume you have a WriteDbContext

                var messages = await dbContext.OutboxMessages
                    .Where(m => m.ProcessedOn == null && m.AttemptCount < 5)
                    .OrderBy(m => m.OccurredOn)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        // Deserialize and process (e.g., Publish via MediatR or send to a Queue)
                        _logger.LogInformation("Processing Outbox message: {Type}", message.Type);

                        // Simulate successful processing
                        message.ProcessedOn = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        message.AttemptCount++;
                        message.Error = ex.Message;
                        _logger.LogError(ex, "Failed to process Outbox message");
                    }
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox processing error");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Poll every 10 seconds
        }
    }
}
