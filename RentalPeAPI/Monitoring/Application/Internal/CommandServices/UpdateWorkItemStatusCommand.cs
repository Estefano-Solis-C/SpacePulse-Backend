using MediatR;
using System;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

/// <summary>
/// Comando para actualizar el estado de una tarea (WorkItem).
/// Valida que solo el remodelador asignado al espacio puede hacer cambios.
/// Retorna la entidad actualizada o null si no se puede actualizar.
/// </summary>
public record UpdateWorkItemStatusCommand(
    int TaskId,
    string Status,
    Guid RequestingUserId,
    decimal? Price
) : IRequest<RentalPeAPI.Monitoring.Domain.Entities.WorkItem?>;

