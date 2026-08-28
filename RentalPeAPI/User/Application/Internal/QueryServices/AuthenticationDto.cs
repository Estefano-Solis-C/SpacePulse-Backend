namespace RentalPeAPI.User.Application.Internal.QueryServices;


public record AuthenticationDto(
    Guid UserId,
    string FullName,
    string Email,
    string Token
);