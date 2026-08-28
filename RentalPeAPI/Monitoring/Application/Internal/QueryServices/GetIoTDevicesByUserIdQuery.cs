using MediatR;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

/// <summary>
/// Query para obtener todos los dispositivos IoT creados por un usuario específico.
/// </summary>
public record GetIoTDevicesByUserIdQuery(Guid UserId)
    : IRequest<IEnumerable<IoTDevice>>;

