using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Controllers;

/// <summary>
/// Controlador para gestionar notificaciones reactivas del sistema.
/// Expone endpoints para leer la bandeja de alertas y marcar como leídas.
/// 
/// NOTA: No hay endpoints POST/DELETE públicos. Las notificaciones se generan
/// de forma 100% automática y reactiva ante hitos del negocio.
/// </summary>
[ApiController]
[Route("api/v1/monitoring/[controller]")]
[Tags("Notifications")]
[Authorize] 
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET: /api/v1/monitoring/notifications/user
    /// Obtiene todas las notificaciones del usuario autenticado, ordenadas por fecha descendente.
    /// Extrae de forma segura el ID del usuario desde el token JWT (ClaimTypes.NameIdentifier).
    /// </summary>
    [HttpGet("user")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserNotifications()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier válido." });
        try
        {
            var query = new GetNotificationsByUserIdQuery(userId);
            var notifications = await _mediator.Send(query);
            if (!notifications.Any())
                return Ok(new List<NotificationResource>()); 

            var resources = notifications.Select(n => new NotificationResource(
                n.Id,
                n.SpaceId,
                n.Title,
                n.Message,
                n.IsRead,
                n.CreatedAt
            )).ToList();

            return Ok(resources);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al obtener notificaciones del usuario.", details = ex.Message });
        }
    }

    /// <summary>
    /// PUT: /api/v1/monitoring/notifications/{id:long}/read
    /// Marca una notificación como leída.
    /// Valida que el usuario solicitante sea el destinatario de la notificación (protección de seguridad).
    /// </summary>
    [HttpPut("{id:long}/read")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> MarkNotificationAsRead(long id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier válido." });

        try
        {
            var command = new MarkNotificationAsReadCommand(id, userId);
            await _mediator.Send(command);

            return Ok(new { message = "Notificación marcada como leída." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Notificación con ID {id} no encontrada." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error al marcar notificación como leída." });
        }
    }
}

