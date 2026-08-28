using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Manejador de la query GetWorkItemsBySpaceIdQuery.
/// Recupera todas las tareas asociadas a un espacio específico.
/// </summary>
public class GetWorkItemsBySpaceIdQueryHandler : IRequestHandler<GetWorkItemsBySpaceIdQuery, IEnumerable<WorkItemResource>>
{
    private readonly IWorkItemRepository _workItemRepository;

    public GetWorkItemsBySpaceIdQueryHandler(IWorkItemRepository workItemRepository)
    {
        _workItemRepository = workItemRepository;
    }

    public async Task<IEnumerable<WorkItemResource>> Handle(GetWorkItemsBySpaceIdQuery query, CancellationToken cancellationToken)
    {
        var workItems = await _workItemRepository.ListBySpaceIdAsync(query.SpaceId);
        return workItems.Select(wi => new WorkItemResource(
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
    }
}

