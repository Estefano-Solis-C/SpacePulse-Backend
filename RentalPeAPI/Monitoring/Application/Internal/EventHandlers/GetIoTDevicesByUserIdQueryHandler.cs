using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para obtener todos los dispositivos IoT creados por un usuario específico.
/// Simula telemetría continua para cada dispositivo encendido.
/// </summary>
public class GetIoTDevicesByUserIdQueryHandler
    : IRequestHandler<GetIoTDevicesByUserIdQuery, IEnumerable<IoTDevice>>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetIoTDevicesByUserIdQueryHandler(
        IIoTDeviceRepository deviceRepository,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<IoTDevice>> Handle(
        GetIoTDevicesByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        var devices = await _deviceRepository.ListByCreatedByUserIdAsync(query.UserId);
        
        foreach (var device in devices)
        {
            device.GenerateRandomValue();
        }
        await _unitOfWork.CompleteAsync();

        return devices;
    }
}

