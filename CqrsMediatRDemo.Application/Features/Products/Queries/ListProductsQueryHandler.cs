using CqrsMediatRDemo.Application.Features.Products.Dtos;
using CqrsMediatRDemo.Application.Features.Products.Queries;
using CqrsMediatRDemo.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var products = new List<ProductDto>
        {
            new ProductDto(Guid.NewGuid(), "Laptop", "High-end laptop", 1200m, "USD", 50),
            new ProductDto(Guid.NewGuid(), "Phone", "Smartphone", 800m, "USD", 200),
            new ProductDto(Guid.NewGuid(), "PC", "Personal computer", 1000m, "USD", 180),
            new ProductDto(Guid.NewGuid(), "Earpods", "Smart handsfree", 200m, "USD", 30),
            new ProductDto(Guid.NewGuid(), "Smartwatch", "Smartwatch", 320m, "USD", 90),
            new ProductDto(Guid.NewGuid(), "Speakers", "Wireless speaker", 80m, "USD", 20)
        };

        return products;
    }
}