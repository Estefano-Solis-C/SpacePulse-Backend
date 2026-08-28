using MediatR;
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para alternar el estado de encendido/apagado de un dispositivo IoT.
/// Valida que el usuario solicitante sea el creador del dispositivo.
/// </summary>
public class ToggleIoTDevicePowerCommandHandler
    : IRequestHandler<ToggleIoTDevicePowerCommand, IoTDevice>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IPropertyContextFacade _propertyFacade;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleIoTDevicePowerCommandHandler(
        IIoTDeviceRepository deviceRepository,
        IPropertyContextFacade propertyFacade,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _propertyFacade = propertyFacade;
        _unitOfWork = unitOfWork;
    }

    public async Task<IoTDevice> Handle(
        ToggleIoTDevicePowerCommand command,
        CancellationToken cancellationToken)
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
                $"El usuario {command.RequestingUserId} no tiene autorización para alternar el estado del dispositivo {command.DeviceId}.");
        }
        
        var spaceStatus = await _propertyFacade.GetSpaceStatusAsync(device.SpaceId);
        
        if (string.IsNullOrEmpty(spaceStatus) || spaceStatus == "Cancelled")
        {
            throw new InvalidOperationException(
                "Acción denegada: El espacio ha sido cancelado.");
        }
        
        device.TogglePower();
        
        await _unitOfWork.CompleteAsync();

        return device;
    }
}

