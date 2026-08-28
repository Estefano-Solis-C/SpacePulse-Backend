using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Manejador del comando MarkNotificationAsReadCommand.
/// Marca una notificación como leída después de validar que el usuario solicitante
/// es el destinatario de la notificación (protección de seguridad).
/// </summary>
public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Unit>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationAsReadCommandHandler(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(MarkNotificationAsReadCommand command, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.FindByIdAsync(command.NotificationId);
        if (notification == null)
            throw new KeyNotFoundException($"Notificación con ID {command.NotificationId} no encontrada.");
        
        if (notification.UserId != command.RequestingUserId)
            throw new UnauthorizedAccessException(
                $"El usuario {command.RequestingUserId} no tiene permisos para marcar como leída la notificación. " +
                $"Solo el destinatario (UserId: {notification.UserId}) puede hacerlo.");
        
        notification.MarkAsRead();
        
        await _unitOfWork.CompleteAsync();

        return Unit.Value;
    }
}

