using MediatR;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

/// <summary>
/// Comando para eliminar un dispositivo IoT.
/// Requiere validación estricta: only el CreatedByUserId del dispositivo puede eliminarlo.
/// </summary>
public record DeleteIoTDeviceCommand(
    long DeviceId,
    Guid RequestingUserId
) : IRequest<Unit>;

