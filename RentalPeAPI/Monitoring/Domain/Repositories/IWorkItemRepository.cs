// Monitoring/Domain/Repositories/IWorkItemRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using RentalPeAPI.Monitoring.Domain.Entities;

namespace RentalPeAPI.Monitoring.Domain.Repositories;

public interface IWorkItemRepository
{
    Task AddAsync(WorkItem workItem);
    Task<WorkItem?> FindByIdAsync(int id);
    Task<IEnumerable<WorkItem>> ListBySpaceIdAsync(long spaceId);
    Task DeleteAsync(int id);
    Task<decimal> SumPricesBySpaceIdAsync(long spaceId);
}
