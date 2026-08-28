using MediatR;

namespace RentalPeAPI.User.Application.Internal.CommandServices;

public record RegisterUserCommand(
    string FullName,
    string Email,
    string Password,
    string? Phone,
    string Role = "customer",
    string? Photo = null

) : IRequest<UserDto>;