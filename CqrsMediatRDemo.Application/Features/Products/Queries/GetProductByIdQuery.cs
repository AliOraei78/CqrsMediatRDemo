using CqrsMediatRDemo.Application.Features.Products.Dtos;
using MediatR;
using System;

namespace CqrsMediatRDemo.Application.Features.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;