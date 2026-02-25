using CqrsMediatRDemo.Application.Features.Products.Dtos;
using CqrsMediatRDemo.Application.Features.Products.Queries;
using CqrsMediatRDemo.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    // private readonly IReadOnlyProductRepository _repository;  ← will be added later

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        // Simulation – in reality, Repository + EF Core will be used
        // Assume a product with this Id exists
        var mockProduct = new Product(
            Guid.Parse("00000000-0000-0000-0000-000000000001"),
            "Sample Product",
            "This is a test product",
            new CqrsMediatRDemo.Domain.ValueObjects.Money(99.99m, "USD"),
            100
        );

        if (mockProduct.Id != request.Id)
            return null;

        return new ProductDto(
            mockProduct.Id,
            mockProduct.Name,
            mockProduct.Description,
            mockProduct.Price.Amount,
            mockProduct.Price.Currency,
            mockProduct.StockQuantity
        );
    }
}


/*
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly ReadDbContext _context;

    public GetProductByIdQueryHandler(ReadDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price.Amount,
                p.Price.Currency,
                p.StockQuantity
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
*/