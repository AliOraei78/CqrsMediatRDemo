using CqrsMediatRDemo.Application.Features.Products.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CqrsMediatRDemo.Application.Interfaces.Repositories;

public interface IProductReadRepository
{
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProductDto>> GetListAsync(
    int page = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default);

}