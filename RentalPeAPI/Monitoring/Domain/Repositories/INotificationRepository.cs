
using RentalPeAPI.Monitoring.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RentalPeAPI.Monitoring.Domain.Repositories;

/// <summary>
/// Repositorio para la entidad Notification.
/// Define operaciones de persistencia alineadas con DDD.
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// Agrega una nueva notificación de forma asíncrona.
    /// </summary>
    Task AddAsync(Notification notification);

    /// <summary>
    /// Busca una notificación por su ID.
    /// </summary>
    Task<Notification?> FindByIdAsync(long id);

    /// <summary>
    /// Lista todas las notificaciones asociadas a un espacio (Space).
    /// </summary>
    Task<IEnumerable<Notification>> ListBySpaceIdAsync(long spaceId);

    /// <summary>
    /// Lista todas las notificaciones destinadas a un usuario específico,
    /// ordenadas por fecha de creación descendente (más recientes primero).
    /// </summary>
    Task<IEnumerable<Notification>> ListByUserIdAsync(Guid userId);
}

