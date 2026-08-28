namespace RentalPeAPI.Property.Interfaces.Rest.Resources
{
    public class SpaceResource
    {
        public long Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Location { get; set; } = "";
        public Guid HomeownerId { get; set; }
        public Guid? RemodelerId { get; set; }
        public string SpaceType { get; set; } = "";
        public decimal DimensionsSquareMeters { get; set; }
        public decimal EstimatedBudget { get; set; }
        public decimal EndingPricing { get; set; } = 0m;
        public string Currency { get; set; } = "PEN";
        public bool HasIot { get; set; }
        public List<string> Images { get; set; } = new();
        public string Status { get; set; } = "";
        public DateTimeOffset PublishedAt { get; set; }
        public DateTimeOffset? AcceptedAt { get; set; }
    }
}