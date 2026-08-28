using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Repositories;

public class WorkItemRepository : IWorkItemRepository
{
    private readonly AppDbContext _context;

    public WorkItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(WorkItem workItem)
    {
        await _context.Tasks.AddAsync(workItem);
    }

    public async Task<WorkItem?> FindByIdAsync(int id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async Task<IEnumerable<WorkItem>> ListBySpaceIdAsync(long spaceId)
    {
        return await _context.Tasks
            .Where(t => t.SpaceId == spaceId)
            .ToListAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var workItem = await _context.Tasks.FindAsync(id);
        if (workItem != null)
        {
            _context.Tasks.Remove(workItem);
        }
    }

    public async Task<decimal> SumPricesBySpaceIdAsync(long spaceId)
    {
        return await _context.Tasks
            .Where(t => t.SpaceId == spaceId)
            .SumAsync(t => t.Price);
    }
}
