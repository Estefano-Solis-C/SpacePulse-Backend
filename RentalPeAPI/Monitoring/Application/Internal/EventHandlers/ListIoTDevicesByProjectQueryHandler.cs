using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para listar dispositivos IoT en un espacio con simulación continua de telemetría.
/// Para cada dispositivo encendido, se genera un nuevo valor de telemetría antes de retornar.
/// </summary>
public class ListIoTDevicesBySpaceQueryHandler
    : IRequestHandler<ListIoTDevicesBySpaceQuery, IEnumerable<IoTDevice>>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ListIoTDevicesBySpaceQueryHandler(
        IIoTDeviceRepository deviceRepository,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<IoTDevice>> Handle(
        ListIoTDevicesBySpaceQuery query,
        CancellationToken cancellationToken)
    {
        var devices = await _deviceRepository.ListBySpaceIdAsync(query.SpaceId);
        
        foreach (var device in devices)
        {
            device.GenerateRandomValue();
        }
        
        await _unitOfWork.CompleteAsync();

        return devices;
    }
}