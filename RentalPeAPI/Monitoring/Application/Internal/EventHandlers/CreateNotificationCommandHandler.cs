using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Manejador del comando CreateNotificationCommand.
/// Crea una nueva alerta de forma asíncrona y la persiste en la base de datos.
/// Utilizado por otros bounded contexts para generar notificaciones de forma reactiva.
/// </summary>
public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, long>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNotificationCommandHandler(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<long> Handle(CreateNotificationCommand command, CancellationToken cancellationToken)
    {
        var notification = new Notification(
            command.UserId,
            command.SpaceId,
            command.Title,
            command.Message
        );
        await _notificationRepository.AddAsync(notification);
        await _unitOfWork.CompleteAsync();
        
        return notification.Id;
    }
}

