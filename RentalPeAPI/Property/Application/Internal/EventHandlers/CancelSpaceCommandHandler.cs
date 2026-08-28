using RentalPeAPI.Property.Application.Internal.CommandServices;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;
using RentalPeAPI.Monitoring.Application.ACL;

namespace RentalPeAPI.Property.Application.Internal.EventHandlers;

/// <summary>
/// Handler para el comando CancelSpaceCommand.
/// Ejecuta la cancelación de un espacio y automáticamente apaga todos los dispositivos IoT asociados.
/// </summary>
public class CancelSpaceCommandHandler
{
    private readonly ISpaceRepository _spaceRepository;
    private readonly IMonitoringContextFacade _monitoringFacade;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSpaceCommandHandler(
        ISpaceRepository spaceRepository,
        IMonitoringContextFacade monitoringFacade,
        IUnitOfWork unitOfWork)
    {
        _spaceRepository = spaceRepository;
        _monitoringFacade = monitoringFacade;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Procesa la cancelación del espacio, apaga dispositivos IoT y despacha notificaciones bifurcadas.
    /// Notifica tanto al Homeowner como al Remodelador (si está asignado).
    /// </summary>
    public async Task<bool> HandleAsync(CancelSpaceCommand request)
    {
        var space = await _spaceRepository.FindByIdAsync(request.SpaceId);
        if (space == null)
            return false;
        
        var remodelerId = space.RemodelerId;
        
        space.CancelProject(request.RequestingUserId);
        
        await _unitOfWork.CompleteAsync();
        
        await _monitoringFacade.DispatchNotificationAsync(
            space.HomeownerId,
            request.SpaceId,
            "Solicitud Cancelada",
            $"El proyecto ha sido cancelado exitosamente. Todos los sensores vinculados han sido desactivados."
        );
        
        if (remodelerId.HasValue)
        {
            await _monitoringFacade.DispatchNotificationAsync(
                remodelerId.Value,
                request.SpaceId,
                "Proyecto Cancelado",
                "El propietario ha cancelado el proyecto en el que estabas asignado. Los sensores vinculados han sido desactivados."
            );
        }
        
        await _monitoringFacade.DisableAllDevicesForSpaceAsync(request.SpaceId);

        return true;
    }
}

