using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.ProductImages.Commands;
using Evaluacion.IA.Application.UseCases.ProductImages.Queries;
using Evaluacion.IA.Application.UseCases.Products.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evaluacion.IA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProductImagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductImagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todas las imágenes de un producto
    /// </summary>
    /// <param name="productId">ID del producto</param>
    /// <returns>Lista de imágenes del producto</returns>
    [HttpGet("product/{productId:int}")]
    [ProducesResponseType(typeof(ApiResponse<List<ProductImageDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<ProductImageDto>>>> GetProductImages(int productId)
    {
        var query = new GetProductImagesQuery(productId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return StatusCode(500, result);
    }

    /// <summary>
    /// Establece una imagen como primaria
    /// </summary>
    /// <param name="id">ID de la imagen</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("{id:int}/set-primary")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> SetPrimaryImage(int id)
    {
        var command = new SetPrimaryImageCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return result.Message.Contains("no encontrado") ? NotFound(result) : StatusCode(500, result);
    }

    /// <summary>
    /// Agrega una nueva imagen a un producto
    /// </summary>
    /// <param name="addImageDto">Datos de la imagen a agregar</param>
    /// <returns>Imagen agregada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductImageDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ProductImageDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ProductImageDto>>> AddProductImage([FromBody] AddProductImageDto addImageDto)
    {
        var command = new AddProductImageCommand(
            addImageDto.ProductId,
            addImageDto.ImageUrl,
            addImageDto.Alt,
            addImageDto.Order
        );

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetProductImages), new { productId = addImageDto.ProductId }, result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Actualiza una imagen de producto existente
    /// </summary>
    /// <param name="id">ID de la imagen</param>
    /// <param name="updateImageDto">Datos actualizados de la imagen</param>
    /// <returns>Imagen actualizada</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductImageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProductImageDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ProductImageDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ProductImageDto>>> UpdateProductImage(int id, [FromBody] UpdateProductImageDto updateImageDto)
    {
        if (id != updateImageDto.Id)
        {
            return BadRequest(ApiResponse<ProductImageDto>.Failure("El ID de la URL no coincide con el ID de la imagen"));
        }

        var command = new UpdateProductImageCommand(
            updateImageDto.Id,
            updateImageDto.ImageUrl,
            updateImageDto.Alt,
            updateImageDto.Order,
            updateImageDto.IsPrimary
        );

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return result.Message.Contains("no encontrado") ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Elimina una imagen de producto
    /// </summary>
    /// <param name="id">ID de la imagen a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProductImage(int id)
    {
        var command = new DeleteProductImageCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return result.Message.Contains("no encontrado") ? NotFound(result) : StatusCode(500, result);
    }

    /// <summary>
    /// Reordena las imágenes de un producto
    /// </summary>
    /// <param name="productId">ID del producto</param>
    /// <param name="reorderDto">Nuevos órdenes de las imágenes</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("product/{productId:int}/reorder")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> ReorderProductImages(int productId, [FromBody] ReorderImagesDto reorderDto)
    {
        var command = new ReorderProductImagesCommand(
            productId,
            reorderDto.ImageOrders
        );

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}

// DTOs específicos para ProductImages
public class AddProductImageDto
{
    public int ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsPrimary { get; set; } = false;
}

public class UpdateProductImageDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsPrimary { get; set; }
}

public class ReorderImagesDto
{
    public List<ImageOrderDto> ImageOrders { get; set; } = new();
}
