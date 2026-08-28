using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para apagar automáticamente todos los dispositivos IoT vinculados a un espacio.
/// Se ejecuta cuando un proyecto se cancela para optimizar el uso de recursos.
/// </summary>
public class TurnOffDevicesBySpaceIdCommandHandler : IRequestHandler<TurnOffDevicesBySpaceIdCommand, Unit>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TurnOffDevicesBySpaceIdCommandHandler(
        IIoTDeviceRepository deviceRepository,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(TurnOffDevicesBySpaceIdCommand command, CancellationToken cancellationToken)
    {
        var devices = await _deviceRepository.ListBySpaceIdAsync(command.SpaceId);
        
        foreach (var device in devices)
        {
            device.TurnOff();
        }
        
        await _unitOfWork.CompleteAsync();

        return Unit.Value;
    }
}

