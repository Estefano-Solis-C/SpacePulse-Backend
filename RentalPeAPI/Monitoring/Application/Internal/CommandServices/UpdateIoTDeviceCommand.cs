using MediatR;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

/// <summary>
/// Comando para actualizar nombre y número de serie de un dispositivo IoT.
/// </summary>
public record UpdateIoTDeviceCommand(
    long DeviceId,
    string Name,
    string? SerialNumber
) : IRequest<Unit>;

