using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Manejador de la query GetNotificationsByUserIdQuery.
/// Obtiene todas las notificaciones del usuario ordenadas por fecha descendente.
/// </summary>
public class GetNotificationsByUserIdQueryHandler : IRequestHandler<GetNotificationsByUserIdQuery, IEnumerable<Notification>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetNotificationsByUserIdQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<Notification>> Handle(GetNotificationsByUserIdQuery query, CancellationToken cancellationToken)
    {
        return await _notificationRepository.ListByUserIdAsync(query.UserId);
    }
}

