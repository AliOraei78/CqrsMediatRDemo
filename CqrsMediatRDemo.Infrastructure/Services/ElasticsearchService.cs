using CqrsMediatRDemo.Application.Features.Products.Dtos;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CqrsMediatRDemo.Infrastructure.Services;

public class ElasticsearchService
{
    private readonly ElasticsearchClient _client;
    private const string ProductsIndex = "products-read-v1";
    public ElasticsearchClient GetClient() => _client;

    public ElasticsearchService(IConfiguration configuration)
    {
        var uri = new Uri(configuration["Elasticsearch:Url"] ?? "http://localhost:9200");
        var settings = new ElasticsearchClientSettings(uri)
            .DefaultIndex(ProductsIndex)
            .Authentication(new BasicAuthentication("elastic", configuration["Elasticsearch:Password"] ?? "changeme"));

        _client = new ElasticsearchClient(settings);
    }

    public async Task<bool> IndexExistsAsync()
    {
        var response = await _client.Indices.ExistsAsync(ProductsIndex);
        return response.Exists;
    }

    public async Task CreateIndexIfNotExistsAsync()
    {
        // Check if index already exists
        if (await IndexExistsAsync())
            return;

        var createResponse = await _client.Indices.CreateAsync<ProductReadModel>(ProductsIndex, c => c
            .Mappings(m => m
                .Properties(p => p
                    .Keyword(k => k.Id)
                    // Added "keyword" as the sub-field name to fix CS0201 and runtime errors
                    .Text(t => t.Name, text => text.Fields(f => f.Keyword("keyword", kw => { })))
                    .Text(t => t.Description)
                    .DoubleNumber(n => n.PriceAmount)
                    .Keyword(k => k.PriceCurrency)
                    .IntegerNumber(n => n.StockQuantity)
                    .Date(d => d.LastUpdated)
                )
            )
        ); // Ensure the semicolon is here

        if (!createResponse.IsValidResponse)
        {
            // Debug info if creation fails
            throw new Exception($"Failed to create index: {createResponse.DebugInformation}");
        }
    }

    // Simple test method – will be used later for indexing
    public async Task IndexTestDocumentAsync()
    {
        var testDoc = new ProductReadModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Laptop",
            Description = "High performance laptop for developers",
            PriceAmount = 45000000m,
            PriceCurrency = "IRR",
            StockQuantity = 15,
            LastUpdated = DateTime.UtcNow
        };

        var response = await _client.IndexAsync(testDoc, idx => idx.Index(ProductsIndex).Id(testDoc.Id.ToString()));

        if (!response.IsValidResponse)
        {
            throw new Exception($"Indexing failed: {response.DebugInformation}");
        }
    }
}