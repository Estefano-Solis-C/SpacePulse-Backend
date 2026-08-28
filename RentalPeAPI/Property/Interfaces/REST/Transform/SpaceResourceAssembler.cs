using RentalPeAPI.Property.Application.Internal.Dtos;
using RentalPeAPI.Property.Interfaces.Rest.Resources;

namespace RentalPeAPI.Property.Interfaces.Rest.Transform
{
    public static class SpaceResourceAssembler
    {
        public static SpaceResource ToResource(SpaceDto dto)
        {
            if (dto == null) return null!;

            return new SpaceResource
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                HomeownerId = dto.HomeownerId,
                RemodelerId = dto.RemodelerId,
                SpaceType = dto.SpaceType,
                DimensionsSquareMeters = dto.DimensionsSquareMeters,
                EstimatedBudget = dto.EstimatedBudget,
                EndingPricing = dto.EndingPricing,
                Currency = dto.Currency,
                HasIot = dto.HasIot,
                Status = dto.Status,
                Images = dto.Images ?? new List<string>(),
                PublishedAt = dto.PublishedAt,
                AcceptedAt = dto.AcceptedAt
            };
        }
    }
}