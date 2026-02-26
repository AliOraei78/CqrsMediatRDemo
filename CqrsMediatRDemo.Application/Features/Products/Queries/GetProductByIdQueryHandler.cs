using CqrsMediatRDemo.Application.Features.Products.Dtos;
using CqrsMediatRDemo.Application.Features.Products.Queries;
using CqrsMediatRDemo.Application.Interfaces.Repositories;
using CqrsMediatRDemo.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CqrsMediatRDemo.Application.Features.Products.Queries;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductReadRepository _readRepository;

    public GetProductByIdQueryHandler(IProductReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _readRepository.GetByIdAsync(request.Id, cancellationToken);
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