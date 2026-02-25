using CqrsMediatRDemo.Domain.Entities;
using CqrsMediatRDemo.Domain.ValueObjects;
using CqrsMediatRDemo.Application.Features.Products.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

// We will add the Repository later – for now, this is a simple mock
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    // private readonly IProductRepository _repository;  ← will be added later

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

        // await _repository.AddAsync(product, cancellationToken);
        // await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        // For now, just simulate the behavior
        Console.WriteLine($"Product created: {product.Name} with Id {productId}");

        return productId;
    }
}