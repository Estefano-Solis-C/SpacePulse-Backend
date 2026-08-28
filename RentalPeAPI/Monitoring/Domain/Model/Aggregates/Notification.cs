using System;

namespace RentalPeAPI.Monitoring.Domain.Entities;

/// <summary>
/// Entidad de dominio Notification que representa una alerta automática generada
/// ante hitos clave del negocio (aceptación de ofertas, tareas completadas, umbrales IoT).
/// 
/// Sigue los principios DDD con encapsulamiento strict y métodos de dominio.
/// </summary>
public class Notification
{

    public long Id { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public long SpaceId { get; private set; }
    
    public string Title { get; private set; } = string.Empty;
    
    public string Message { get; private set; } = string.Empty;
    
    public bool IsRead { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    private Notification() { }

    /// <summary>
    /// Constructor principal para crear una nueva notificación.
    /// Encápsula la lógica de inicialización con valores por defecto.
    /// </summary>
    /// <param name="userId">ID del usuario destinatario (Guid)</param>
    /// <param name="spaceId">ID de la obra/espacio asociado</param>
    /// <param name="title">Título descriptivo de la alerta</param>
    /// <param name="message">Mensaje detallado de la alerta</param>
    public Notification(Guid userId, long spaceId, string title, string message)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId debe ser un GUID válido", nameof(userId));
        if (spaceId <= 0)
            throw new ArgumentException("SpaceId debe ser > 0", nameof(spaceId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title es obligatorio", nameof(title));
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message es obligatorio", nameof(message));

        UserId = userId;
        SpaceId = spaceId;
        Title = title;
        Message = message;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Método de dominio para marcar la notificación como leída.
    /// Cumple con la regla de negocio: una vez leída, permanece así.
    /// </summary>
    public void MarkAsRead()
    {
        IsRead = true;
    }
}
