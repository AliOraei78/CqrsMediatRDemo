using CqrsMediatRDemo.Application.Features.Products.Commands;
using CqrsMediatRDemo.Application.Interfaces;
using CqrsMediatRDemo.Application.Interfaces.Repositories;
using CqrsMediatRDemo.Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CqrsMediatRDemo.Tests.Unit;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateProductCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateProductAndReturnId()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Test Product",
            Description: "Description",
            PriceAmount: 100m,
            Currency: "USD",
            InitialStock: 50
        );

        Product? capturedProduct = null;

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((p, _) => capturedProduct = p);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        capturedProduct.Should().NotBeNull();
        capturedProduct!.Name.Should().Be("Test Product");
        capturedProduct.Price.Amount.Should().Be(100m);
        capturedProduct.StockQuantity.Should().Be(50);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NegativePrice_ShouldThrowInDomain()
    {
        // Arrange
        var command = new CreateProductCommand(
            "Invalid Product",
            "Desc",
            -10m,
            "USD",
            10
        );

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }
}
