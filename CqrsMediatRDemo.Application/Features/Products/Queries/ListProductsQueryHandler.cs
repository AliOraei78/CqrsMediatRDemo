using CqrsMediatRDemo.Application.Features.Products.Dtos;
using CqrsMediatRDemo.Application.Features.Products.Queries;
using CqrsMediatRDemo.Application.Interfaces.Repositories;
using CqrsMediatRDemo.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CqrsMediatRDemo.Application.Features.Products.Queries;

public class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, List<ProductDto>>
{
    private readonly IProductReadRepository _readRepository;

    public ListProductsQueryHandler(IProductReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<List<ProductDto>> Handle(
        ListProductsQuery request,
        CancellationToken cancellationToken)
    {
        return await _readRepository.GetListAsync(
            page: request.Page,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);
    }
}