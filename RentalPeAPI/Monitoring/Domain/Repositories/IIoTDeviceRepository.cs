using RentalPeAPI.Monitoring.Domain.Model.Aggregates;

namespace RentalPeAPI.Monitoring.Domain.Repositories;

public interface IIoTDeviceRepository
{
    Task AddAsync(IoTDevice device);
    Task<IoTDevice?> FindByIdAsync(long id);
    Task<IEnumerable<IoTDevice>> ListBySpaceIdAsync(long spaceId);
    Task<IEnumerable<IoTDevice>> ListByCreatedByUserIdAsync(Guid createdByUserId);
    void Remove(IoTDevice device);
}

