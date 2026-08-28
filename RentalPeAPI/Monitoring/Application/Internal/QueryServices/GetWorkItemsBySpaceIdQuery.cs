using MediatR;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

/// <summary>
/// Query para obtener todas las tareas (WorkItems) asociadas a un espacio específico.
/// </summary>
public record GetWorkItemsBySpaceIdQuery(long SpaceId)
    : IRequest<IEnumerable<WorkItemResource>>;

