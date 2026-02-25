namespace CqrsMediatRDemo.Application.Features.Products.Dtos;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal PriceAmount,
    string Currency,
    int StockQuantity
);