using MediatR;
using System;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

/// <summary>
/// Comando para crear una nueva notificación de forma automática.
/// Se despacha internamente ante hitos clave del negocio.
/// No es expuesto públicamente en REST, es 100% interno y reactivo.
/// </summary>
public record CreateNotificationCommand(
    Guid UserId,
    long SpaceId,
    string Title,
    string Message
) : IRequest<long>;

