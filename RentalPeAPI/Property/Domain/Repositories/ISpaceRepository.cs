using RentalPeAPI.Property.Domain.Aggregates;

namespace RentalPeAPI.Property.Domain.Repositories;

public interface ISpaceRepository
{
    Task AddAsync(Space space);
    Task<Space?> FindByIdAsync(long id);

    Task<IEnumerable<Space>> ListAsync();
    void Remove(Space space);
  
}