using RentalPeAPI.Property.Application.Internal.CommandServices;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;
using RentalPeAPI.Monitoring.Application.ACL;

namespace RentalPeAPI.Property.Application.Internal.EventHandlers;

/// <summary>
/// Handler para el comando CompleteSpaceCommand.
/// Ejecuta la completación de un proyecto y despacha notificaciones
/// reactivas tanto al Homeowner como al Remodelador (si aplica).
/// </summary>
public class CompleteSpaceCommandHandler
{
    private readonly ISpaceRepository _spaceRepository;
    private readonly IMonitoringContextFacade _monitoringFacade;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteSpaceCommandHandler(
        ISpaceRepository spaceRepository,
        IMonitoringContextFacade monitoringFacade,
        IUnitOfWork unitOfWork)
    {
        _spaceRepository = spaceRepository;
        _monitoringFacade = monitoringFacade;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Procesa la completación del proyecto y despacha notificaciones bifurcadas.
    /// Notifica tanto al Homeowner como al Remodelador (si está asignado).
    /// </summary>
    public async Task<bool> HandleAsync(CompleteSpaceCommand request)
    {

        var space = await _spaceRepository.FindByIdAsync(request.SpaceId);
        if (space == null)
            return false;
        space.CompleteProject(request.RequestingUserId);
        await _unitOfWork.CompleteAsync();
        
        await _monitoringFacade.DispatchNotificationAsync(
            space.HomeownerId,
            request.SpaceId,
            "Proyecto Finalizado",
            "Has marcado la obra como finalizada con éxito."
        );
        
        if (space.RemodelerId.HasValue)
        {
            await _monitoringFacade.DispatchNotificationAsync(
                space.RemodelerId.Value,
                request.SpaceId,
                "Proyecto Finalizado",
                "El propietario ha dado la conformidad final de la obra. ¡Excelente trabajo!"
            );
        }

        return true;
    }
}

