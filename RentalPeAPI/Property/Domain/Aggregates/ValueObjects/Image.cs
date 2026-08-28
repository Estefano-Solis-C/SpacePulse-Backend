namespace RentalPeAPI.Property.Domain.Aggregates.ValueObjects;

public record Image
{
    public string Url { get; }
    public Image(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Image URL cannot be empty.", nameof(url));
        Url = url;
    }

    public override string ToString() => Url;
}