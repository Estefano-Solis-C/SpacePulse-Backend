using Microsoft.EntityFrameworkCore;
using RentalPeAPI.Property.Domain.Aggregates;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace RentalPeAPI.Property.Infrastructure.Persistence.EFC.Repositories;

public class SpaceRepository : ISpaceRepository
{
    private readonly AppDbContext _context;

    public SpaceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Space space)
    {
        await _context.Spaces.AddAsync(space);
    }

    public async Task<Space?> FindByIdAsync(long id)
    {
        return await _context.Spaces
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Space>> ListAsync()
    {
        return await _context.Spaces
            .ToListAsync();
    }

    public void Remove(Space space)
    {
        _context.Spaces.Remove(space);
    }
}