using MediatR;
using System;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

/// <summary>
/// Comando para marcar una notificación como leída.
/// Valida que el usuario solicitante sea el destinatario de la notificación.
/// </summary>
public record MarkNotificationAsReadCommand(
    long NotificationId,
    Guid RequestingUserId
) : IRequest<Unit>;

