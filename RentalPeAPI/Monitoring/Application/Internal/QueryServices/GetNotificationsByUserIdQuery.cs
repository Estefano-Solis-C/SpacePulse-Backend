using MediatR;
using RentalPeAPI.Monitoring.Domain.Entities;
using System;
using System.Collections.Generic;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

/// <summary>
/// Query para obtener todas las notificaciones destinadas a un usuario específico,
/// ordenadas por fecha de creación descendente (más recientes primero).
/// </summary>
public record GetNotificationsByUserIdQuery(Guid UserId) 
    : IRequest<IEnumerable<Notification>>;

