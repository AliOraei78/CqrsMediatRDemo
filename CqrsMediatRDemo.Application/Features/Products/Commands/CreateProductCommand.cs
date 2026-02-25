using CqrsMediatRDemo.Domain.Entities;
using CqrsMediatRDemo.Domain.ValueObjects;
using MediatR;
using System;

namespace CqrsMediatRDemo.Application.Features.Products.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal PriceAmount,
    string Currency,
    int InitialStock
) : IRequest<Guid>;