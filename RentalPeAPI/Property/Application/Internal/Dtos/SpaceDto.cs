
namespace RentalPeAPI.Property.Application.Internal.Dtos;

public class SpaceDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public Guid HomeownerId { get; set; }
    public Guid? RemodelerId { get; set; }
    public string SpaceType { get; set; } = string.Empty;
    public decimal DimensionsSquareMeters { get; set; }
    public decimal EstimatedBudget { get; set; }
    public decimal EndingPricing { get; set; } = 0m;
    public string Currency { get; set; } = "PEN";
    public string Status { get; set; } = "Published";
    public bool HasIot { get; set; }
    public List<string> Images { get; set; } = new();
    public DateTimeOffset PublishedAt { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
}