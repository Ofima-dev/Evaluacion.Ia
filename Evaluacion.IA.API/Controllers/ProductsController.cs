using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using Evaluacion.IA.Application.UseCases.Products.Commands;
using Evaluacion.IA.Application.UseCases.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evaluacion.IA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todos los productos del sistema
    /// </summary>
    /// <returns>Lista de productos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductSummaryDto>>>> GetAllProducts()
    {
        var query = new GetAllProductsQuery();
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return StatusCode(500, result);
    }

    /// <summary>
    /// Obtiene un producto por su ID
    /// </summary>
    /// <param name="id">ID del producto</param>
    /// <returns>Producto encontrado</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProductById(int id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    /// <summary>
    /// Obtiene productos por categoría
    /// </summary>
    /// <param name="categoryId">ID de la categoría</param>
    /// <returns>Lista de productos de la categoría</returns>
    [HttpGet("category/{categoryId:int}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductSummaryDto>>>> GetProductsByCategory(int categoryId)
    {
        var query = new GetProductsByCategoryQuery(categoryId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return StatusCode(500, result);
    }

    /// <summary>
    /// Busca productos por términos de búsqueda
    /// </summary>
    /// <param name="searchTerm">Término de búsqueda</param>
    /// <returns>Lista de productos que coinciden con la búsqueda</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductSummaryDto>>>> SearchProducts([FromQuery] string searchTerm)
    {
        var query = new SearchProductsQuery(searchTerm);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return StatusCode(500, result);
    }

    /// <summary>
    /// Crea un nuevo producto
    /// </summary>
    /// <param name="createProductDto">Datos del producto a crear</param>
    /// <returns>Producto creado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        var command = new CreateProductCommand(
            createProductDto.Sku,
            createProductDto.Name,
            createProductDto.Description,
            createProductDto.Price,
            createProductDto.Currency,
            createProductDto.CategoryId,
            createProductDto.Image,
            "Img"
        );

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetProductById), new { id = result.Data!.Id }, result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Actualiza un producto existente
    /// </summary>
    /// <param name="id">ID del producto</param>
    /// <param name="updateProductDto">Datos actualizados del producto</param>
    /// <returns>Producto actualizado</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
    {
        if (id != updateProductDto.Id)
        {
            return BadRequest(ApiResponse<ProductDto>.Failure("El ID de la URL no coincide con el ID del producto"));
        }

        var command = new UpdateProductCommand(
            updateProductDto.Id,
            updateProductDto.Name,
            updateProductDto.Description,
            updateProductDto.Price,
            updateProductDto.Currency,
            updateProductDto.CategoryId,
            updateProductDto.IsActive
        );

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return result.Message.Contains("no encontrado") ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Elimina un producto
    /// </summary>
    /// <param name="id">ID del producto a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(int id)
    {
        var command = new DeleteProductCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return result.Message.Contains("no encontrado") ? NotFound(result) : StatusCode(500, result);
    }
}
