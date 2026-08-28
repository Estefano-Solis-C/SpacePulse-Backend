using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para obtener un dispositivo IoT por su ID.
/// </summary>
public class GetIoTDeviceByIdQueryHandler
    : IRequestHandler<GetIoTDeviceByIdQuery, IoTDevice?>
{
    private readonly IIoTDeviceRepository _deviceRepository;

    public GetIoTDeviceByIdQueryHandler(IIoTDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<IoTDevice?> Handle(
        GetIoTDeviceByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await _deviceRepository.FindByIdAsync(query.DeviceId);
    }
}

