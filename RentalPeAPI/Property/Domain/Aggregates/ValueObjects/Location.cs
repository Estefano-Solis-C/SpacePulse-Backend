namespace RentalPeAPI.Property.Domain.Aggregates.ValueObjects;

public record Location
{
    public string Address { get; }
    public Location(string address)
    {
        Address = string.IsNullOrWhiteSpace(address) ? "N/A" : address;
    }
    public override string ToString() => Address;
}
