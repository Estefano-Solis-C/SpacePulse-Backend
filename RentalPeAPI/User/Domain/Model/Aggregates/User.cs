namespace RentalPeAPI.User.Domain;

public class User
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }

    public string? Phone { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Role { get; private set; } = "customer";
    public string? Photo { get; private set; }

    public List<PaymentMethod> PaymentMethods { get; private set; } = new();

    private User() { }

    public User(Guid id, string fullName, string email, string passwordHash,
        string? phone = null, string role = "customer", string? photo = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email no puede estar vacío.", nameof(email));

        Id = id;
        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        Phone = phone;
        CreatedAt = DateTime.UtcNow;
        Role = string.IsNullOrWhiteSpace(role) ? "customer" : role;
        Photo = photo;
    }

    public void AddPaymentMethod(Guid id, string type, string number, string expiry, string cvv)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Number is required.", nameof(number));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type is required.", nameof(type));
        if (string.IsNullOrWhiteSpace(expiry))
            throw new ArgumentException("Expiry is required.", nameof(expiry));

        string lastFour = number.Length >= 4 ? number.Substring(number.Length - 4) : number;
        var paymentMethod = new PaymentMethod(id, this.Id, type, lastFour, expiry);
        PaymentMethods.Add(paymentMethod);
    }
}