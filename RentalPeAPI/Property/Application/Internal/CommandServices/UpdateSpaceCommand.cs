namespace RentalPeAPI.Property.Application.Internal.CommandServices
{
    public class UpdateSpaceCommand
    {
        public long Id { get; }
        public string Title { get; }
        public string Description { get; }
        public string Location { get; }
        public decimal DimensionsSquareMeters { get; }
        public decimal EstimatedBudget { get; }
        public bool HasIot { get; }
        public List<string>? Images { get; }

        public UpdateSpaceCommand(
            long id,
            string title,
            string description,
            string location,
            decimal dimensionsSquareMeters,
            decimal estimatedBudget,
            bool hasIot,
            List<string>? images = null)
        {
            Id = id;
            Title = title;
            Description = description;
            Location = location;
            DimensionsSquareMeters = dimensionsSquareMeters;
            EstimatedBudget = estimatedBudget;
            HasIot = hasIot;
            Images = images;
        }
    }
}