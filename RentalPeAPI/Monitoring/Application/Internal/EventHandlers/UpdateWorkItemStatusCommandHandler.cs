using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para el comando UpdateWorkItemStatusCommand.
/// Valida que solo el remodelador asignado al espacio pueda actualizar el estado de la tarea.
/// </summary>
public class UpdateWorkItemStatusCommandHandler : IRequestHandler<UpdateWorkItemStatusCommand, WorkItem?>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly ISpaceRepository _spaceRepository;
    private readonly IPropertyContextFacade _propertyFacade;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateWorkItemStatusCommandHandler(
        IWorkItemRepository workItemRepository,
        ISpaceRepository spaceRepository,
        IPropertyContextFacade propertyFacade,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _workItemRepository = workItemRepository;
        _spaceRepository = spaceRepository;
        _propertyFacade = propertyFacade;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<WorkItem?> Handle(UpdateWorkItemStatusCommand command, CancellationToken cancellationToken)
    {
        var workItem = await _workItemRepository.FindByIdAsync(command.TaskId);
        if (workItem == null)
            throw new KeyNotFoundException($"WorkItem con ID {command.TaskId} no encontrado.");
        
        var space = await _spaceRepository.FindByIdAsync(workItem.SpaceId);
        if (space == null)
            throw new KeyNotFoundException($"Space con ID {workItem.SpaceId} no encontrado.");
        
        if (space.RemodelerId == null || space.RemodelerId != command.RequestingUserId)
            throw new UnauthorizedAccessException(
                $"El usuario {command.RequestingUserId} no tiene permisos para cambiar el estado de la tarea. " +
                $"Solo el remodelador asignado al espacio (RemodelerId: {space.RemodelerId}) puede hacerlo.");
        
        var spaceStatus = await _propertyFacade.GetSpaceStatusAsync(workItem.SpaceId);
        
        if (string.IsNullOrEmpty(spaceStatus) || 
            (spaceStatus != "Published" && spaceStatus != "Accepted"))
        {
            throw new InvalidOperationException(
                "No se pueden modificar tareas: El espacio está completado o cancelado.");
        }
        
        bool transitioningToCompleted = !workItem.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase) 
                                       && command.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase);
        
        workItem.UpdateProgress(command.Status, workItem.PlannedStartDate, workItem.PlannedEndDate);

        if (command.Price.HasValue)
        {
            workItem.UpdatePrice(command.Price.Value);
            var workItems = await _workItemRepository.ListBySpaceIdAsync(workItem.SpaceId);
            var totalPricing = workItems.Sum(item => item.Price);
            await _propertyFacade.UpdateSpaceTotalPricingAsync(workItem.SpaceId, totalPricing);
        }
        
        await _unitOfWork.CompleteAsync();
        
         if (transitioningToCompleted)
         {
             var notificationCommandHomeowner = new CreateNotificationCommand(
                 space.HomeownerId,
                 space.Id,
                 "Hito Completado",
                 $"Se ha completado con éxito la tarea: '{workItem.Title}'."
             );
             await _mediator.Send(notificationCommandHomeowner);
             
             var notificationCommandRemodeler = new CreateNotificationCommand(
                 space.RemodelerId.Value,
                 space.Id,
                 "Tarea Completada",
                 $"La tarea '{workItem.Title}' ha sido marcada como completada. ¡Excelente trabajo!"
             );
             await _mediator.Send(notificationCommandRemodeler);
         }
         
        return workItem;
    }
}
