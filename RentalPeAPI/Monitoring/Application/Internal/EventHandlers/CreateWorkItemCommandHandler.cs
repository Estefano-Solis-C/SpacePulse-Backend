using MediatR;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Shared.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Manejador del comando CreateWorkItemCommand.
/// Crea una nueva tarea (WorkItem) y la persiste en la base de datos.
/// </summary>
public class CreateWorkItemCommandHandler : IRequestHandler<CreateWorkItemCommand, int>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IPropertyContextFacade _propertyFacade;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWorkItemCommandHandler(
        IWorkItemRepository workItemRepository,
        IPropertyContextFacade propertyFacade,
        IUnitOfWork unitOfWork)
    {
        _workItemRepository = workItemRepository;
        _propertyFacade = propertyFacade;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateWorkItemCommand command, CancellationToken cancellationToken)
    {
        var spaceStatus = await _propertyFacade.GetSpaceStatusAsync(command.SpaceId);
        
        if (string.IsNullOrEmpty(spaceStatus) || 
            (spaceStatus != "Published" && spaceStatus != "Accepted"))
        {
            throw new InvalidOperationException(
                "No se pueden modificar tareas: El espacio está completado o cancelado.");
        }
        var workItem = new WorkItem(
            command.SpaceId,
            command.CreatedByUserId,
            command.Title,
            command.Description,
            command.PhotoUrl,
            command.PlannedStartDate,
            command.PlannedEndDate,
            price: command.Price
        );
        
        await _workItemRepository.AddAsync(workItem);
        await _unitOfWork.CompleteAsync();
        
        if (command.Price > 0)
        {
            var totalPricing = await _workItemRepository.SumPricesBySpaceIdAsync(workItem.SpaceId);
            await _propertyFacade.UpdateSpaceTotalPricingAsync(workItem.SpaceId, totalPricing);
        }
        
        return workItem.Id;
    }
}