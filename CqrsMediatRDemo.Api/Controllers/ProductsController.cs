using CqrsMediatRDemo.Application.Features.Products.Commands;
using CqrsMediatRDemo.Application.Features.Products.Dtos;
using CqrsMediatRDemo.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CqrsMediatRDemo.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="command">Product information</param>
    /// <returns>The created product ID + link to details</returns>
    /// <response code="201">Product created successfully</response>
    /// <response code="400">Invalid input</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var productId = await _sender.Send(command);

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = productId },
            new { Id = productId, Message = "Product created successfully" });
    }

    /// <summary>
    /// Get product details by ID
    /// </summary>
    /// <param name="id">Product ID (GUID)</param>
    /// <returns>Product details or NotFound</returns>
    /// <response code="200">Product found</response>
    /// <response code="404">Product not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        var product = await _sender.Send(new GetProductByIdQuery(id));

        if (product is null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found" });
        }

        return Ok(product);
    }

    /// <summary>
    /// Get a paginated list of products
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10)</param>
    /// <returns>List of products</returns>
    /// <response code="200">List of products</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductDto>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(new { Message = "Page and pageSize must be positive integers" });
        }

        var products = await _sender.Send(new ListProductsQuery(page, pageSize));

        return Ok(products);
    }
}
