using System.Collections.Generic;
using System.Linq;
using MediatR;
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Manejador de la query GetWorkItemsByUserIdQuery.
/// Obtiene todas las tareas asociadas a un usuario en espacios donde es propietario o remodelador asignado.
/// Utiliza la Anti-Corruption Layer (PropertyContextFacade) para obtener los espacios del usuario.
/// </summary>
public class GetWorkItemsByUserIdQueryHandler : IRequestHandler<GetWorkItemsByUserIdQuery, IEnumerable<WorkItemResource>>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IPropertyContextFacade _propertyContextFacade;

    public GetWorkItemsByUserIdQueryHandler(
        IWorkItemRepository workItemRepository,
        IPropertyContextFacade propertyContextFacade)
    {
        _workItemRepository = workItemRepository;
        _propertyContextFacade = propertyContextFacade;
    }

    public async Task<IEnumerable<WorkItemResource>> Handle(GetWorkItemsByUserIdQuery query, CancellationToken cancellationToken)
    {
        var spaceIds = (await _propertyContextFacade.GetSpaceIdsByUserIdAsync(query.UserId)).ToList();
        if (!spaceIds.Any())
            return new List<WorkItemResource>();
        var userWorkItems = new List<WorkItemResource>();
        foreach (var spaceId in spaceIds)
        {
            var workItems = await _workItemRepository.ListBySpaceIdAsync(spaceId);
            var resources = workItems.Select(wi => new WorkItemResource(
                wi.Id,
                wi.SpaceId,
                wi.CreatedByUserId,
                wi.Title,
                wi.Description,
                wi.PhotoUrl,
                wi.PlannedStartDate,
                wi.PlannedEndDate,
                wi.Price,
                wi.Status,
                wi.CreatedAt,
                wi.CompletedAt
            )).ToList();

            userWorkItems.AddRange(resources);
        }

        return userWorkItems;
    }
}

