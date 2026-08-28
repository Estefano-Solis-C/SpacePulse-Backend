namespace RentalPeAPI.User.Application.Internal.CommandServices;

public record PaymentMethodDto(
    Guid Id,
    string Type,
    string LastFourDigits,
    string Expiry
);

public record UserDto(
    Guid Id,
    string FullName,
    string Email,
    string? Phone,
    DateTime CreatedAt,
    string Role,
    string? Photo,
    IReadOnlyList<PaymentMethodDto> PaymentMethods
);