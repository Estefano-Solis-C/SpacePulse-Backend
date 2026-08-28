using RentalPeAPI.Property.Domain.Aggregates;

namespace RentalPeAPI.Property.Domain.Services;

public interface ISpaceDomainService
{
    bool ValidateAvailability(Space space, DateTime startTime, DateTime endTime);
}