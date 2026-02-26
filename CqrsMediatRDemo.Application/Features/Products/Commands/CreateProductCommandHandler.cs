using CqrsMediatRDemo.Application.Features.Products.Commands;
using CqrsMediatRDemo.Application.Interfaces;
using CqrsMediatRDemo.Application.Interfaces.Repositories;
using CqrsMediatRDemo.Domain.Entities;
using CqrsMediatRDemo.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CqrsMediatRDemo.Application.Features.Products.Commands;

// We will add the Repository later – for now, this is a simple mock
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    // private readonly IProductRepository _repository;  ← will be added later
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var productId = Guid.NewGuid();

        var price = new Money(request.PriceAmount, request.Currency);

        var product = new Product(
            id: productId,
            name: request.Name,
            description: request.Description,
            price: price,
            initialStock: request.InitialStock
        );

        await _repository.AddAsync(product, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return productId;
    }
}