using MediatR;
using System.Collections.Generic;
using RentalPeAPI.Monitoring.Domain.Entities;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

/// <summary>
/// Query para listar notificaciones asociadas a un espacio específico.
/// </summary>
public record ListNotificationsQuery(long SpaceId) 
    : IRequest<IEnumerable<Notification>>;