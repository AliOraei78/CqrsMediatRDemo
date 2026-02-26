using CqrsMediatRDemo.Application.Features.Products.Dtos;
using CqrsMediatRDemo.Application.Interfaces.Repositories;
using CqrsMediatRDemo.Infrastructure.Services;
using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Text;

namespace CqrsMediatRDemo.Infrastructure.Persistence.ReadRepositories;

public class ProductReadRepository : IProductReadRepository
{
    private readonly ElasticsearchService _esService;

    public ProductReadRepository(ElasticsearchService esService)
    {
        _esService = esService ?? throw new ArgumentNullException(nameof(esService));
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var client = _esService.GetClient();

        var response = await client.GetAsync<ProductReadModel>(
            id.ToString(),
            g => g.Index("products-read-v1"),
            cancellationToken);

        if (!response.IsValidResponse || !response.Found || response.Source == null)
            return null;

        var readModel = response.Source;

        return new ProductDto(
            readModel.Id,
            readModel.Name,
            readModel.Description,
            readModel.PriceAmount,
            readModel.PriceCurrency,
            readModel.StockQuantity
        );
    }

    public async Task<List<ProductDto>> GetListAsync(
    int page = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
    {
        var client = _esService.GetClient();

        var from = (page - 1) * pageSize;

        var response = await client.SearchAsync<ProductReadModel>(s => s
                .From(from)
                .Size(pageSize)
                .Sort(sort => sort
                    .Field(f => f.LastUpdated, f => f.Order(SortOrder.Desc))
                ),
                cancellationToken);

        if (!response.IsValidResponse)
        {
            return new List<ProductDto>();
        }

        var products = response.Documents.Select(readModel => new ProductDto(
            readModel.Id,
            readModel.Name,
            readModel.Description,
            readModel.PriceAmount,
            readModel.PriceCurrency,
            readModel.StockQuantity
        )).ToList();

        return products;
    }
}
