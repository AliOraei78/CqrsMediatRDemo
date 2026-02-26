using CqrsMediatRDemo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CqrsMediatRDemo.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}