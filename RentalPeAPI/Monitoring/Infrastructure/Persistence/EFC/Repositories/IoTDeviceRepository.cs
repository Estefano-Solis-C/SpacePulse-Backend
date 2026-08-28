using Microsoft.EntityFrameworkCore;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Repositories;

public class IoTDeviceRepository : IIoTDeviceRepository
{
    private readonly AppDbContext _context;

    public IoTDeviceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(IoTDevice device)
    {
        await _context.IoTDevices.AddAsync(device);
    }

    public async Task<IoTDevice?> FindByIdAsync(long id)
    {
        return await _context.IoTDevices.FindAsync(id);
    }

    public async Task<IEnumerable<IoTDevice>> ListBySpaceIdAsync(long spaceId)
    {
        return await _context.IoTDevices
            .Where(d => d.SpaceId == spaceId)
            .ToListAsync();
    }

    public async Task<IEnumerable<IoTDevice>> ListByCreatedByUserIdAsync(Guid createdByUserId)
    {
        return await _context.IoTDevices
            .Where(d => d.CreatedByUserId == createdByUserId)
            .ToListAsync();
    }

    public void Remove(IoTDevice device)
    {
        _context.IoTDevices.Remove(device);
    }
}