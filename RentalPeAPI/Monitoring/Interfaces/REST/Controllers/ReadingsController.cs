// Monitoring/Interfaces/REST/Controllers/ReadingsController.cs
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;
using RentalPeAPI.Shared.Domain.Repositories;
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Property.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Controllers;

/// <summary>
/// Controlador refactorizado para gestionar lecturas de telemetría de dispositivos IoT.
/// Ya no usa tabla de Readings. Consulta directamente los IoTDevices y simula telemetría.
/// PASO 5C: Despacha notificaciones automáticamente ante anomalías telemétricas.
/// </summary>
[ApiController]
[Route("api/v1/monitoring/[controller]")]
[Tags("Readings")]
[Authorize] 
public class ReadingsController : ControllerBase
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IPropertyContextFacade _propertyFacade;
    private readonly ISpaceRepository _spaceRepository;

    public ReadingsController(
        IIoTDeviceRepository deviceRepository, 
        IUnitOfWork unitOfWork, 
        IMediator mediator,
        IPropertyContextFacade propertyFacade,
        ISpaceRepository spaceRepository)
    {
        _deviceRepository = deviceRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _propertyFacade = propertyFacade;
        _spaceRepository = spaceRepository;
    }

    /// <summary>
    /// Método privado helper para extraer propiedades privadas del agregado IoTDevice.
    /// Utiliza Reflection para acceder a MinThreshold, MaxThreshold e IsInAlertState
    /// que son gestionadas internamente por el dominio.
    /// </summary>
    private static (decimal MinThreshold, decimal MaxThreshold, bool IsInAlertState) ExtractPrivateProperties(object device)
    {
        var minThresholdProp = device.GetType().GetProperty("MinThreshold", BindingFlags.NonPublic | BindingFlags.Instance);
        var maxThresholdProp = device.GetType().GetProperty("MaxThreshold", BindingFlags.NonPublic | BindingFlags.Instance);
        var isInAlertStateProp = device.GetType().GetProperty("IsInAlertState", BindingFlags.NonPublic | BindingFlags.Instance);

        var minThreshold = minThresholdProp?.GetValue(device) as decimal? ?? 0m;
        var maxThreshold = maxThresholdProp?.GetValue(device) as decimal? ?? 100m;
        var isInAlertState = isInAlertStateProp?.GetValue(device) as bool? ?? false;

        return (minThreshold, maxThreshold, isInAlertState);
    }

    /// <summary>
    /// Método privado helper para validar umbrales y disparar notificación automática
    /// si se detecta una anomalía telemetría (PASO 5C).
    /// 
    /// Despacha notificaciones bifurcadas tanto al Homeowner como al Remodeler (si aplica).
    /// </summary>
    private async Task ValidateThresholdsAndDispatchNotificationAsync(
        RentalPeAPI.Monitoring.Domain.Model.Aggregates.IoTDevice device)
    {
        var (minThreshold, maxThreshold) = device.GetThresholds();
        bool isCurrentlyInAlert = device.Value < minThreshold || device.Value > maxThreshold;
        
        if (isCurrentlyInAlert)
        {
            device.UpdateAlertState(true);
            await _unitOfWork.CompleteAsync();
            
            var spaceUsers = await _propertyFacade.GetSpaceUsersAsync(device.SpaceId);
            
            if (spaceUsers.HasValue)
            {
                var (ownerId, remodelerId) = spaceUsers.Value;
                
                string alertTitle = $"Alerta Crítica IoT: {device.Name}";
                string alertMessage = $"El sensor '{device.Name}' ha detectado una lectura anómala. " +
                    $"Métrica: {device.MetricName}, Valor: {device.Value:F2} {device.Unit}, " +
                    $"Rango permitido: {minThreshold:F2} - {maxThreshold:F2}.";
                
                var notificationCommandHomeowner = new CreateNotificationCommand(
                    ownerId,
                    device.SpaceId,
                    alertTitle,
                    alertMessage
                );
                await _mediator.Send(notificationCommandHomeowner);
                
                if (remodelerId.HasValue)
                {
                    var notificationCommandRemodeler = new CreateNotificationCommand(
                        remodelerId.Value,
                        device.SpaceId,
                        alertTitle,
                        alertMessage
                    );
                    await _mediator.Send(notificationCommandRemodeler);
                }
            }
        }
    }




    /// <summary>
    /// GET: /api/v1/monitoring/readings/device/{deviceId}
    /// Devuelve el detalle completo de un dispositivo específico con telemetría, umbrales y estado de alerta.
    /// Antes de retornar, invoca GenerateRandomValue() y valida umbrales para disparar alertas automáticas.
    /// </summary>
    [HttpGet("device/{deviceId:long}")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetDeviceDetail(long deviceId)
    {
        // Validar que el usuario autenticado tenga un NameIdentifier válido
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
            // Recuperar el dispositivo
            var device = await _deviceRepository.FindByIdAsync(deviceId);
            if (device == null)
                return NotFound(new { error = $"Dispositivo con ID {deviceId} no encontrado." });

            // Invocar simulación de telemetría
            device.GenerateRandomValue();

            // PASO 5C: Validar umbrales y disparar notificación automática si es necesario
            await ValidateThresholdsAndDispatchNotificationAsync(device);

            // Persistir cambios
            await _unitOfWork.CompleteAsync();

            // Extraer propiedades privadas usando Reflection
            var (minThreshold, maxThreshold, isInAlertState) = ExtractPrivateProperties(device);

            // Mapear a DTO extendido con toda la información
            var resource = new IoTDeviceDetailExtendedResource(
                device.Id,
                device.SpaceId,
                device.Type,
                device.Name,
                device.SerialNumber,
                device.MetricName,
                device.Unit,
                device.Value,
                device.Timestamp,
                device.IsOn,
                minThreshold,
                maxThreshold,
                isInAlertState
            );

            return Ok(resource);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al obtener detalle del dispositivo.", details = ex.Message });
        }
    }


    /// <summary>
    /// GET: /api/v1/monitoring/readings/space/{spaceId}
    /// Devuelve la lista de detalles de todos los dispositivos de un espacio con telemetría, umbrales y estado de alerta.
    /// Valida umbrales y dispara notificaciones automáticas ante anomalías (PASO 5C).
    /// </summary>
    [HttpGet("space/{spaceId:long}")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetSpaceReadings(long spaceId)
    {
        // Validar que el usuario autenticado tenga un NameIdentifier válido
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
            // Recuperar dispositivos del espacio
            var devices = await _deviceRepository.ListBySpaceIdAsync(spaceId);
            if (!devices.Any())
                return NotFound(new { error = $"No se encontraron dispositivos para el espacio {spaceId}." });

            // Invocar simulación de telemetría para cada dispositivo
            foreach (var device in devices)
            {
                device.GenerateRandomValue();
                // PASO 5C: Validar umbrales y disparar notificación automática si es necesario
                await ValidateThresholdsAndDispatchNotificationAsync(device);
            }

            // Persistir cambios
            await _unitOfWork.CompleteAsync();

            // Mapear a DTOs extendidos con toda la información
            var resources = devices.Select(d =>
            {
                var (minThreshold, maxThreshold, isInAlertState) = ExtractPrivateProperties(d);
                return new IoTDeviceDetailExtendedResource(
                    d.Id,
                    d.SpaceId,
                    d.Type,
                    d.Name,
                    d.SerialNumber,
                    d.MetricName,
                    d.Unit,
                    d.Value,
                    d.Timestamp,
                    d.IsOn,
                    minThreshold,
                    maxThreshold,
                    isInAlertState
                );
            }).ToList();

            return Ok(resources);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al obtener lecturas del espacio.", details = ex.Message });
        }
    }

    /// <summary>
    /// GET: /api/v1/monitoring/readings/user
    /// Extrae el ID del usuario del token JWT y devuelve los detalles de todos sus dispositivos con telemetría, umbrales y estado de alerta.
    /// Valida umbrales y dispara notificaciones automáticas ante anomalías (PASO 5C).
    /// </summary>
    [HttpGet("user")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserReadings()
    {
        // Validar que el usuario autenticado tenga un NameIdentifier válido
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier válido." });

        try
        {
            // Recuperar dispositivos del usuario
            var devices = await _deviceRepository.ListByCreatedByUserIdAsync(userId);
            if (!devices.Any())
                return NotFound(new { error = "No se encontraron dispositivos para el usuario autenticado." });

            // Invocar simulación de telemetría para cada dispositivo
            foreach (var device in devices)
            {
                device.GenerateRandomValue();
                // PASO 5C: Validar umbrales y disparar notificación automática si es necesario
                await ValidateThresholdsAndDispatchNotificationAsync(device);
            }

            // Persistir cambios
            await _unitOfWork.CompleteAsync();

            // Mapear a DTOs extendidos con toda la información
            var resources = devices.Select(d =>
            {
                var (minThreshold, maxThreshold, isInAlertState) = ExtractPrivateProperties(d);
                return new IoTDeviceDetailExtendedResource(
                    d.Id,
                    d.SpaceId,
                    d.Type,
                    d.Name,
                    d.SerialNumber,
                    d.MetricName,
                    d.Unit,
                    d.Value,
                    d.Timestamp,
                    d.IsOn,
                    minThreshold,
                    maxThreshold,
                    isInAlertState
                );
            }).ToList();

            return Ok(resources);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al obtener lecturas del usuario.", details = ex.Message });
        }
    }
}