using CqrsMediatRDemo.Application.Interfaces.Repositories;
using CqrsMediatRDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CqrsMediatRDemo.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly WriteDbContext _context;

    public ProductRepository(WriteDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
