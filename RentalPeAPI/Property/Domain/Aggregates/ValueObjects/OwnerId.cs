namespace RentalPeAPI.Property.Domain.Aggregates.ValueObjects;

public record OwnerId
{
    public long Value { get; }
    public OwnerId(long value)
    {
        if (value <= 0)
            throw new ArgumentException("OwnerId must be positive.", nameof(value));
        Value = value;
    }
    public override string ToString() => Value.ToString();
}