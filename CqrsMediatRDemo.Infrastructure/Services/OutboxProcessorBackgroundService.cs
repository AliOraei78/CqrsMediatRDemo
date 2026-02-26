using CqrsMediatRDemo.Application.Features.Products.Dtos;
using CqrsMediatRDemo.Domain.Events;
using CqrsMediatRDemo.Domain.ValueObjects;
using CqrsMediatRDemo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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
                var dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>(); // ← added
                var esService = scope.ServiceProvider.GetRequiredService<ElasticsearchService>();

                var messages = await dbContext.OutboxMessages
                    .Where(m => m.ProcessedOn == null && m.AttemptCount < 5)
                    .OrderBy(m => m.OccurredOn)
                    .Take(20) // larger batch for better performance
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        // Polymorphic deserialization to DomainEvent
                        var eventType = Type.GetType(message.Type);
                        if (eventType == null || !typeof(DomainEvent).IsAssignableFrom(eventType))
                        {
                            throw new InvalidOperationException($"Unknown event type: {message.Type}");
                        }

                        var domainEvent = JsonSerializer.Deserialize(message.Payload, eventType) as DomainEvent;
                        if (domainEvent == null)
                            throw new InvalidOperationException("Deserialization failed");

                        // Publish to MediatR (for existing Handlers)
                        await mediator.Publish(domainEvent, stoppingToken);

                        // Update Read Model in Elasticsearch based on event type
                        await UpdateReadModelAsync(esService, domainEvent, stoppingToken);

                        message.ProcessedOn = DateTime.UtcNow;
                        _logger.LogInformation("Processed Outbox message: {Type} - {Id}", message.Type, message.Id);
                    }
                    catch (Exception ex)
                    {
                        message.AttemptCount++;
                        message.Error = ex.Message;
                        _logger.LogError(ex, "Failed processing Outbox {Id} - Attempt {Attempt}", message.Id, message.AttemptCount);
                    }
                }

                if (messages.Any())
                    await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox processor cycle error");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // reduced polling interval for faster testing
        }
    }

    private async Task UpdateReadModelAsync(
        ElasticsearchService esService,
        DomainEvent domainEvent,
        CancellationToken ct)
    {
        var client = esService.GetClient();

        switch (domainEvent)
        {
            case ProductCreatedEvent evt:
                var readModel = new ProductReadModel
                {
                    Id = evt.ProductId,
                    Name = evt.Name,
                    Description = evt.Description,
                    PriceAmount = evt.Price.Amount,
                    PriceCurrency = evt.Price.Currency,
                    StockQuantity = evt.InitialStock,
                    LastUpdated = DateTime.UtcNow
                };

                var indexResponse = await client.IndexAsync(
                    readModel,
                    idx => idx
                        .Index("products-read-v1")
                        .Id(readModel.Id.ToString()),
                    ct);

                if (!indexResponse.IsValidResponse)
                {
                    throw new Exception(
                        $"Failed to index new product {readModel.Id} in Elasticsearch: {indexResponse.DebugInformation}");
                }

                _logger.LogInformation("Indexed new product in Elasticsearch: {ProductId}", readModel.Id);
                break;

            case ProductPriceChangedEvent evt:
                var updateResponse = await client.UpdateAsync<ProductReadModel, object>(
                        "products-read-v1",       // Argument 1: Index Name
                        evt.ProductId.ToString(), // Argument 2: Document Id
                        u => u                    // Argument 3: Action/Lambda
                            .Doc(new
                            {
                                PriceAmount = evt.NewPrice.Amount,
                                PriceCurrency = evt.NewPrice.Currency,
                                LastUpdated = DateTime.UtcNow
                            })
                            .RetryOnConflict(3),
                        ct);

                if (!updateResponse.IsValidResponse)
                {
                    throw new Exception(
                        $"Failed to update price for product {evt.ProductId} in Elasticsearch: {updateResponse.DebugInformation}");
                }

                _logger.LogInformation("Updated product price in Elasticsearch: {ProductId}", evt.ProductId);
                break;

            default:
                _logger.LogWarning("No Read Model update handler for event type: {EventType}", domainEvent.GetType().Name);
                break;
        }
    }

    private async Task UpdateProductPriceInEsAsync(ElasticsearchService esService, Guid productId, Money newPrice, CancellationToken ct)
    {
        var client = esService.GetClient(); // Add this new method in ElasticsearchService or expose _client

        var response = await client.UpdateAsync<ProductReadModel, object>(
                index: "products-read-v1",
                id: productId.ToString(),
                u => u
                    .Doc(new
                    {
                        PriceAmount = newPrice.Amount,
                        PriceCurrency = newPrice.Currency,
                        LastUpdated = DateTime.UtcNow
                    })
                    .RetryOnConflict(3),
                ct);

        if (!response.IsValidResponse)
        {
            throw new Exception($"Failed to update product {productId} in ES: {response.DebugInformation}");
        }
    }
}
