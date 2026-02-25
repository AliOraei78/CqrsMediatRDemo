using CqrsMediatRDemo.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CqrsMediatRDemo.Application.Features.Products.Events;

public class ProductPriceChangedEventHandler : INotificationHandler<ProductPriceChangedEvent>
{
    private readonly ILogger<ProductPriceChangedEventHandler> _logger;

    public ProductPriceChangedEventHandler(ILogger<ProductPriceChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Domain Event: Product price changed. ProductId: {ProductId}, Old: {OldPrice}, New: {NewPrice}",
            notification.ProductId,
            notification.OldPrice.Amount,
            notification.NewPrice.Amount
        );

        return Task.CompletedTask;
    }
}