using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration; 

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Repositorio EF Core para la entidad Notification.
/// Implementa las operaciones de persistencia definidas en INotificationRepository.
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public async Task<Notification?> FindByIdAsync(long id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<IEnumerable<Notification>> ListBySpaceIdAsync(long spaceId)
    {
        return await _context.Notifications
            .Where(n => n.SpaceId == spaceId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> ListByUserIdAsync(Guid userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
}