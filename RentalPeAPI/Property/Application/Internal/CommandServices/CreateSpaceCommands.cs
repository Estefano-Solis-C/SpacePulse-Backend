namespace RentalPeAPI.Property.Application.Internal.CommandServices
{
    public class CreateSpaceCommand
    {
        public Guid HomeownerId { get; }
        public string Title { get; }
        public string Description { get; }
        public string Location { get; }
        public string SpaceType { get; }
        public decimal DimensionsSquareMeters { get; }
        public decimal EstimatedBudget { get; }
        public string Currency { get; }
        public bool HasIot { get; }
        public List<string> Images { get; }

        public CreateSpaceCommand(
            Guid homeownerId,
            string title,
            string description,
            string location,
            string spaceType,
            decimal dimensionsSquareMeters,
            decimal estimatedBudget,
            string currency,
            bool hasIot,
            List<string> images)
        {
            HomeownerId = homeownerId;
            Title = title;
            Description = description;
            Location = location;
            SpaceType = spaceType;
            DimensionsSquareMeters = dimensionsSquareMeters;
            EstimatedBudget = estimatedBudget;
            Currency = currency;
            HasIot = hasIot;
            Images = images ?? new();
        }
    }
}