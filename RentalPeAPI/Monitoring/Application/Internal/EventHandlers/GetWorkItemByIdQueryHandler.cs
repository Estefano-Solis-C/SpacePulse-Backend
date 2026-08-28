using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Manejador de la query GetWorkItemByIdQuery.
/// Recupera una tarea completa y la mapea a WorkItemResource.
/// </summary>
public class GetWorkItemByIdQueryHandler : IRequestHandler<GetWorkItemByIdQuery, WorkItemResource?>
{
    private readonly IWorkItemRepository _workItemRepository;

    public GetWorkItemByIdQueryHandler(IWorkItemRepository workItemRepository)
    {
        _workItemRepository = workItemRepository;
    }

    public async Task<WorkItemResource?> Handle(GetWorkItemByIdQuery query, CancellationToken cancellationToken)
    {
        var workItem = await _workItemRepository.FindByIdAsync(query.TaskId);

        if (workItem == null)
            return null;
        
        return new WorkItemResource(
            workItem.Id,
            workItem.SpaceId,
            workItem.CreatedByUserId,
            workItem.Title,
            workItem.Description,
            workItem.PhotoUrl,
            workItem.PlannedStartDate,
            workItem.PlannedEndDate,
            workItem.Price,
            workItem.Status,
            workItem.CreatedAt,
            workItem.CompletedAt
        );
    }
}

