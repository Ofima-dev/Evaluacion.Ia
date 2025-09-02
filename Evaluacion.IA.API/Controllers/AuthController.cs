using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Auth.Commands;
using Evaluacion.IA.Application.UseCases.Users.Commands;
using Evaluacion.IA.Application.UseCases.Users.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Evaluacion.IA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema
    /// </summary>
    /// <param name="createUserDto">Datos del usuario a crear</param>
    /// <returns>Usuario creado</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] CreateUserDto createUserDto)
    {
        var command = new CreateUserCommand
        {
            Email = createUserDto.Email,
            Password = createUserDto.Password,
            RoleId = createUserDto.RoleId
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(Register), result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Inicia sesión de un usuario
    /// </summary>
    /// <param name="loginDto">Credenciales de login</param>
    /// <returns>Token JWT y datos del usuario</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto loginDto)
    {
        var command = new LoginCommand
        {
            Email = loginDto.Email,
            Password = loginDto.Password
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return Unauthorized(result);
    }
}

// DTOs específicos para Auth
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
    public DateTime ExpiresAt { get; set; }
}
