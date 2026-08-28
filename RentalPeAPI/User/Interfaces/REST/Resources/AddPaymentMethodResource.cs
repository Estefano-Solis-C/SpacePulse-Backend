namespace RentalPeAPI.User.Interfaces.REST.Resources;

public class AddPaymentMethodResource
{
    public string Type { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Expiry { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
}