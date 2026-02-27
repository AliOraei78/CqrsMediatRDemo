using CqrsMediatRDemo.Application.Features.Products.Dtos;
using CqrsMediatRDemo.Application.Features.Products.Queries;
using CqrsMediatRDemo.Application.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CqrsMediatRDemo.Tests.Unit;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IProductReadRepository> _readRepoMock;
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _readRepoMock = new Mock<IProductReadRepository>();
        _handler = new GetProductByIdQueryHandler(_readRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingId_ShouldReturnProductDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedDto = new ProductDto(id, "Test", "Desc", 100m, "USD", 50);

        _readRepoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _handler.Handle(new GetProductByIdQuery(id), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task Handle_NonExistingId_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        _readRepoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var result = await _handler.Handle(new GetProductByIdQuery(id), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
