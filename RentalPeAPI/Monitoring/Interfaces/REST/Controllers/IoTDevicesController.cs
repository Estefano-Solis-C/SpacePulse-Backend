using System;
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
/// Controlador para dispositivos IoT vinculados a espacios (Spaces) con simulación automática
/// de telemetría, control de encendido/apagado y validaciones de seguridad por usuario creador.
/// </summary>
[ApiController]
[Route("api/v1/monitoring/[controller]")]
[Authorize] 
public class IoTDevicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public IoTDevicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// POST: api/v1/monitoring/iot-devices
    /// Crea un nuevo dispositivo IoT para un espacio específico con autocompletado de métricas.
    /// Solo accesible para usuarios autenticados. El creador se extrae del token JWT.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> CreateDevice([FromBody] CreateIoTDeviceResource resource)
    {
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var createdByUserId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
             var command = new CreateIoTDeviceCommand(
                 SpaceId: resource.SpaceId,
                 CreatedByUserId: createdByUserId,
                 Type: resource.Type,
                 Name: resource.Name,
                 SerialNumber: resource.SerialNumber,
                 CustomMetricName: resource.CustomMetricName,
                 CustomUnit: resource.CustomUnit,
                 CustomMinThreshold: resource.CustomMinThreshold,
                 CustomMaxThreshold: resource.CustomMaxThreshold
             );

            var device = await _mediator.Send(command);

            var deviceResource = new IoTDeviceSummaryResource(
                device.Id,
                device.SpaceId,
                device.Type,
                device.Name,
                device.SerialNumber
            );

            return CreatedAtAction(
                nameof(ListDevicesBySpace),
                new { spaceId = device.SpaceId },
                deviceResource
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// GET: api/v1/monitoring/iot-devices/space/{spaceId}
    /// Lista todos los dispositivos IoT de un espacio específico.
    /// Simula telemetría continua: actualiza valores para dispositivos encendidos.
    /// Solo accesible para usuarios autenticados (ambos roles).
    /// </summary>
    [HttpGet("space/{spaceId:long}")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> ListDevicesBySpace(long spaceId)
    {
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        var query = new ListIoTDevicesBySpaceQuery(spaceId);
        var devices = await _mediator.Send(query);

        var resources = devices.Select(d => new IoTDeviceSummaryResource(
            d.Id,
            d.SpaceId,
            d.Type,
            d.Name,
            d.SerialNumber
        ));

        return Ok(resources);
    }

    /// <summary>
    /// PUT: api/v1/monitoring/iot-devices/{deviceId}
    /// Actualiza nombre y número de serie de un dispositivo IoT.
    /// Solo el usuario creador del dispositivo puede actualizar.
    /// </summary>
    [HttpPut("{deviceId:long}")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateDevice(long deviceId, [FromBody] UpdateIoTDeviceResource resource)
    {
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {

            var device = await _mediator.Send(new GetIoTDeviceByIdQuery(deviceId));
            if (device == null)
                return NotFound(new { error = $"Dispositivo con ID {deviceId} no encontrado." });
            
            if (!Guid.TryParse(userIdClaim, out var userId) || device.CreatedByUserId != userId)
                return Forbid();
            
            device.UpdateDetails(resource.Name, resource.SerialNumber ?? string.Empty);
            
            var updateCommand = new UpdateIoTDeviceCommand(deviceId, resource.Name, resource.SerialNumber);
            await _mediator.Send(updateCommand);

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// PUT: api/v1/monitoring/iot-devices/{deviceId}/toggle
    /// Alterna el estado de encendido/apagado de un dispositivo IoT.
    /// Solo el usuario creador del dispositivo puede togglear.
    /// Si se enciende, se genera inmediatamente un nuevo valor de telemetría.
    /// </summary>
    [HttpPut("{deviceId:long}/toggle")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> TogglePower(long deviceId)
    {
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
            var command = new ToggleIoTDevicePowerCommand(deviceId, userId);
            var device = await _mediator.Send(command);

            var resource = new IoTDeviceSummaryResource(
                device.Id,
                device.SpaceId,
                device.Type,
                device.Name,
                device.SerialNumber
            );

            return Ok(new
            {
                message = $"Dispositivo {(device.IsOn ? "encendido" : "apagado")} exitosamente.",
                data = resource,
                isOn = device.IsOn
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Dispositivo con ID {deviceId} no encontrado." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// GET: api/v1/monitoring/iot-devices/my-devices
    /// Lista todos los dispositivos IoT creados por el usuario autenticado.
    /// Simula telemetría continua: actualiza valores para dispositivos encendidos.
    /// Solo accesible para usuarios autenticados (ambos roles).
    /// </summary>
    [HttpGet("my-devices")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetMyDevices()
    {
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        var query = new GetIoTDevicesByUserIdQuery(userId);
        var devices = await _mediator.Send(query);

        var resources = devices.Select(d => new IoTDeviceSummaryResource(
            d.Id,
            d.SpaceId,
            d.Type,
            d.Name,
            d.SerialNumber
        ));

        return Ok(resources);
    }

    /// <summary>
    /// DELETE: api/v1/monitoring/iot-devices/{id}
    /// Elimina un dispositivo IoT. Solo el creador puede eliminarlo.
    /// Solo accesible para usuarios autenticados.
    /// </summary>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteDevice(long id)
    {
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var requestingUserId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
            var command = new DeleteIoTDeviceCommand(id, requestingUserId);
            await _mediator.Send(command);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Dispositivo con ID {id} no encontrado." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}