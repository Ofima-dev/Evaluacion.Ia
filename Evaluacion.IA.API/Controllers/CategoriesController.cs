using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Evaluacion.IA.Application.UseCases.Categories.Commands;
using Evaluacion.IA.Application.UseCases.Categories.Queries;
using Evaluacion.IA.Application.DTOs;
using Evaluacion.IA.Application.Common;

namespace Evaluacion.IA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todas las categorías del sistema
    /// </summary>
    /// <returns>Lista de categorías</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CategoryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedResult<CategoryDto>>>> GetAllCategories()
    {
        var query = new GetAllCategoriesQuery();
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return StatusCode(500, result);
    }

    /// <summary>
    /// Obtiene las categorías activas para selección
    /// </summary>
    /// <returns>Lista de categorías activas</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<List<CategorySummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<CategorySummaryDto>>>> GetActiveCategories()
    {
        var query = new GetActiveCategoriesQuery();
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return StatusCode(500, result);
    }

    /// <summary>
    /// Obtiene una categoría por su ID
    /// </summary>
    /// <param name="id">ID de la categoría</param>
    /// <returns>Categoría encontrada</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategoryById(int id)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    /// <summary>
    /// Crea una nueva categoría
    /// </summary>
    /// <param name="createCategoryDto">Datos de la categoría a crear</param>
    /// <returns>Categoría creada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        var command = new CreateCategoryCommand(
            createCategoryDto.Name,
            createCategoryDto.Description
        );

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Data!.Id }, result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Actualiza una categoría existente
    /// </summary>
    /// <param name="id">ID de la categoría</param>
    /// <param name="updateCategoryDto">Datos actualizados de la categoría</param>
    /// <returns>Categoría actualizada</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
    {
        if (id != updateCategoryDto.Id)
        {
            return BadRequest(ApiResponse<CategoryDto>.Failure("El ID de la URL no coincide con el ID de la categoría"));
        }

        var command = new UpdateCategoryCommand(
            updateCategoryDto.Id,
            updateCategoryDto.Name,
            updateCategoryDto.Description,
            updateCategoryDto.IsActive
        );

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return result.Message.Contains("no encontrado") ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Elimina una categoría
    /// </summary>
    /// <param name="id">ID de la categoría a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(int id)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        if (result.Message.Contains("no encontrado"))
        {
            return NotFound(result);
        }

        if (result.Message.Contains("productos asociados"))
        {
            return BadRequest(result);
        }

        return StatusCode(500, result);
    }
}
