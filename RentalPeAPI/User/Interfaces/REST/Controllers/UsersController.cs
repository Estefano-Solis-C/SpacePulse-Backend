using System.Security.Claims;
using MediatR; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.User.Application.Internal.CommandServices;
using RentalPeAPI.User.Application.Internal.QueryServices;
using RentalPeAPI.User.Interfaces.REST.Resources;

namespace RentalPeAPI.User.Interfaces.REST.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// POST: Registra un nuevo usuario (Homeowner o Remodeler).
    /// Endpoint público - No requiere autenticación.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous] 
    public async Task<ActionResult<UserDto>> RegisterUser([FromBody] RegisterUserResource resource)
    {
        var command = new RegisterUserCommand(
            resource.FullName,
            resource.Email,
            resource.Password,
            resource.Phone,
            resource.Role,
            resource.Photo
        );
        var userDto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUserById), new { userId = userDto.Id }, userDto);
    }

    /// <summary>
    /// POST: Login de usuario existente.
    /// Endpoint público - No requiere autenticación.
    /// Retorna token JWT si las credenciales son válidas.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous] 
    public async Task<IActionResult> Login([FromBody] LoginResource resource)
    {
        try
        {
            var query = new LoginQuery(resource.Email, resource.Password);
            var authDto = await _mediator.Send(query);
            
            return Ok(authDto); 
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message }); 
        }
    }
    
    /// <summary>
    /// GET: Obtiene información de un usuario específico.
    /// Solo accesible para usuarios autenticados (ambos roles).
    /// </summary>
    [HttpGet("{userId:guid}")]
    [Authorize(Roles = "Homeowner,Remodeler")] 
    public async Task<ActionResult<UserDto>> GetUserById(Guid userId)
    {
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var authenticatedUserId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });
        
        if (authenticatedUserId != userId)
            return Forbid("No tienes permiso para acceder a la información de otro usuario.");

        var query = new GetUserByIdQuery(userId);
        var userDto = await _mediator.Send(query);

        if (userDto is null) return NotFound();

        return Ok(userDto);
    }
    
    /// <summary>
    /// POST: Añade un método de pago a un usuario específico.
    /// Solo accesible para usuarios autenticados (ambos roles).
    /// </summary>
    [HttpPost("{userId:guid}/payment-methods")]
    [Authorize(Roles = "Homeowner,Remodeler")] 
    public async Task<ActionResult<UserDto>> AddPaymentMethod(Guid userId, [FromBody] AddPaymentMethodResource resource)
    {
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var authenticatedUserId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });
        
        if (authenticatedUserId != userId)
            return Forbid("No tienes permiso para modificar la información de otro usuario.");

        var command = new AddPaymentMethodCommand(
            userId,
            resource.Type,
            resource.Number,
            resource.Expiry,
            resource.Cvv
        );

        var userDto = await _mediator.Send(command);
        return Ok(userDto);
    }
}