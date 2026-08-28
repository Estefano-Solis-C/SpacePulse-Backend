using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para eliminar un dispositivo IoT.
/// Valida estrictamente que el usuario solicitante sea el CreatedByUserId del dispositivo.
/// </summary>
public class DeleteIoTDeviceCommandHandler : IRequestHandler<DeleteIoTDeviceCommand, Unit>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteIoTDeviceCommandHandler(
        IIoTDeviceRepository deviceRepository,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteIoTDeviceCommand command, CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.FindByIdAsync(command.DeviceId);
        if (device == null)
        {
            throw new KeyNotFoundException(
                $"Dispositivo IoT con ID {command.DeviceId} no encontrado.");
        }
        
        if (device.CreatedByUserId != command.RequestingUserId)
        {
            throw new UnauthorizedAccessException(
                $"El usuario {command.RequestingUserId} no tiene autorización para eliminar el dispositivo {command.DeviceId}. " +
                $"Solo el creador del dispositivo (CreatedByUserId: {device.CreatedByUserId}) puede eliminarlo.");
        }
        
        _deviceRepository.Remove(device);
        await _unitOfWork.CompleteAsync();

        return Unit.Value;
    }
}

