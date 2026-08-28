using MediatR;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

/// <summary>
/// Query para obtener todas las tareas (WorkItems) asociadas a un usuario.
/// Retorna tareas en espacios donde el usuario es propietario (Homeowner) o remodelador asignado (Remodeler).
/// </summary>
public record GetWorkItemsByUserIdQuery(Guid UserId)
    : IRequest<IEnumerable<WorkItemResource>>;

