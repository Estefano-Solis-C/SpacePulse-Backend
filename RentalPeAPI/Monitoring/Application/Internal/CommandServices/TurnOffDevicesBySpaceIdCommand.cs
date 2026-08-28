using MediatR;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

/// <summary>
/// Comando para apagar todos los dispositivos IoT vinculados a un espacio específico.
/// Se utiliza automáticamente cuando un proyecto se cancela.
/// </summary>
public record TurnOffDevicesBySpaceIdCommand(
    long SpaceId
) : IRequest<Unit>;

