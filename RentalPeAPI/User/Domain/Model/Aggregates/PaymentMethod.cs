namespace RentalPeAPI.User.Domain;

public class PaymentMethod
{
    private PaymentMethod() { }

    public PaymentMethod(Guid id, Guid userId, string type, string lastFourDigits, string expiry)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id is required.", nameof(id));
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type is required.", nameof(type));
        if (string.IsNullOrWhiteSpace(lastFourDigits))
            throw new ArgumentException("LastFourDigits is required.", nameof(lastFourDigits));
        if (string.IsNullOrWhiteSpace(expiry))
            throw new ArgumentException("Expiry is required.", nameof(expiry));

        Id = id;
        UserId = userId;
        Type = type;
        LastFourDigits = lastFourDigits;
        Expiry = expiry;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string LastFourDigits { get; private set; } = string.Empty;
    public string Expiry { get; private set; } = string.Empty;
}

