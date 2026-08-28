using MediatR;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

/// <summary>
/// Query para obtener los detalles completos de una tarea (WorkItem) por su ID.
/// </summary>
public record GetWorkItemByIdQuery(int TaskId)
    : IRequest<WorkItemResource?>;

