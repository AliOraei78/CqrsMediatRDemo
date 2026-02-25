using CqrsMediatRDemo.Application.Features.Products.Dtos;
using MediatR;
using System.Collections.Generic;

namespace CqrsMediatRDemo.Application.Features.Products.Queries;

public record ListProductsQuery(int Page = 1, int PageSize = 10) : IRequest<List<ProductDto>>;