namespace CqrsMediatRDemo.Application.Features.Products.Dtos;

public record ProductReadModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal PriceAmount { get; init; }
    public string PriceCurrency { get; init; } = "USD";
    public int StockQuantity { get; init; }
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
}