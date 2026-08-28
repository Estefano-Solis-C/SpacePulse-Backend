using MediatR;
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para crear un dispositivo IoT con validación de ACL y autocompletado de métricas.
/// Implementa regla estricta: Solo se puede crear dispositivo si el espacio existe y HasIot = true.
/// </summary>
public class CreateIoTDeviceCommandHandler
    : IRequestHandler<CreateIoTDeviceCommand, IoTDevice>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IPropertyContextFacade _propertyFacade;
    private readonly IUnitOfWork _unitOfWork;

    public CreateIoTDeviceCommandHandler(
        IIoTDeviceRepository deviceRepository,
        IPropertyContextFacade propertyFacade,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _propertyFacade = propertyFacade;
        _unitOfWork = unitOfWork;
    }

    public async Task<IoTDevice> Handle(
        CreateIoTDeviceCommand command,
        CancellationToken cancellationToken)
    {
        var spaceHasIot = await _propertyFacade.ValidateSpaceHasIoTEnabledAsync(command.SpaceId);
        if (!spaceHasIot)
        {
            throw new InvalidOperationException(
                $"El espacio con ID {command.SpaceId} no existe o no tiene tecnología IoT habilitada.");
        }
        
        var spaceStatus = await _propertyFacade.GetSpaceStatusAsync(command.SpaceId);
        
        if (string.IsNullOrEmpty(spaceStatus) || spaceStatus == "Cancelled")
        {
            throw new InvalidOperationException(
                "Acción denegada: El espacio ha sido cancelado.");
        }
        
        var device = new IoTDevice(
            command.SpaceId,
            command.CreatedByUserId,
            command.Type,
            command.Name,
            command.SerialNumber ?? string.Empty,
            command.CustomMetricName,
            command.CustomUnit,
            command.CustomMinThreshold,
            command.CustomMaxThreshold
        );

        await _deviceRepository.AddAsync(device);
        await _unitOfWork.CompleteAsync();

        return device;
    }
}