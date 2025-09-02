using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Evaluacion.IA.Application.UseCases.Roles.Commands;
using Evaluacion.IA.Application.UseCases.Roles.Queries;
using Evaluacion.IA.Application.UseCases.Roles.DTOs;
using Evaluacion.IA.Application.Common;

namespace Evaluacion.IA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todos los roles del sistema
    /// </summary>
    /// <returns>Lista de roles</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<RoleDto>>>> GetAllRoles()
    {
        var query = new GetAllRolesQuery();
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return StatusCode(500, result);
    }

    /// <summary>
    /// Obtiene los roles disponibles para asignación
    /// </summary>
    /// <returns>Lista de roles disponibles</returns>
    [HttpGet("available")]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<RoleDto>>>> GetAvailableRoles()
    {
        var query = new GetAvailableRolesQuery();
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return StatusCode(500, result);
    }

    /// <summary>
    /// Obtiene un rol por su ID
    /// </summary>
    /// <param name="id">ID del rol</param>
    /// <returns>Rol encontrado</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetRoleById(int id)
    {
        var query = new GetRoleByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    /// <summary>
    /// Crea un nuevo rol
    /// </summary>
    /// <param name="createRoleDto">Datos del rol a crear</param>
    /// <returns>Rol creado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole([FromBody] CreateRoleDto createRoleDto)
    {
        var command = new CreateRoleCommand
        {
            Description = createRoleDto.Description
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetRoleById), new { id = result.Data!.Id }, result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Actualiza un rol existente
    /// </summary>
    /// <param name="id">ID del rol</param>
    /// <param name="updateRoleDto">Datos actualizados del rol</param>
    /// <returns>Rol actualizado</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RoleDto>>> UpdateRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
    {
        if (id != updateRoleDto.Id)
        {
            return BadRequest(ApiResponse<RoleDto>.Failure("El ID de la URL no coincide con el ID del rol"));
        }

        var command = new UpdateRoleCommand
        {
            Id = updateRoleDto.Id,
            Description = updateRoleDto.Description
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return result.Message.Contains("no encontrado") ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Elimina un rol
    /// </summary>
    /// <param name="id">ID del rol a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteRole(int id)
    {
        var command = new DeleteRoleCommand { Id = id };
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return result.Message.Contains("no encontrado") ? NotFound(result) : StatusCode(500, result);
    }
}

// DTOs específicos para Roles
public class CreateRoleDto
{
    public string Description { get; set; } = string.Empty;
}

public class UpdateRoleDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
}
