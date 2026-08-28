using MediatR;
using System;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

/// <summary>
/// Comando para crear una nueva tarea (WorkItem) vinculada a un espacio.
/// Puede ser ejecutado por el dueño del espacio o por un remodelador.
/// 
/// Incluye soporte para PhotoUrl y fechas planificadas opcionales.
/// </summary>
public record CreateWorkItemCommand(
    long SpaceId,
    Guid CreatedByUserId,
    string Title,
    string Description,
    string? PhotoUrl,
    DateTime? PlannedStartDate,
    DateTime? PlannedEndDate,
    decimal Price = 0
) : IRequest<int>;
