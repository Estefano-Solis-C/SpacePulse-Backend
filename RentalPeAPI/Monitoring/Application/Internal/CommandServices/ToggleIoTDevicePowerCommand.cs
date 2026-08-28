using MediatR;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

/// <summary>
/// Comando para alternar el estado de encendido/apagado de un dispositivo IoT.
/// Incluye validación de seguridad: solo el usuario creador puede alternar el estado.
/// </summary>
public record ToggleIoTDevicePowerCommand(
    long DeviceId,
    Guid RequestingUserId
) : IRequest<IoTDevice>;

