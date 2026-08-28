using MediatR;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

/// <summary>
/// Query para obtener un dispositivo IoT por su ID.
/// </summary>
public record GetIoTDeviceByIdQuery(long DeviceId)
    : IRequest<IoTDevice?>;

