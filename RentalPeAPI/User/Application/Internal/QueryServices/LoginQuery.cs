using MediatR;

namespace RentalPeAPI.User.Application.Internal.QueryServices;

public record LoginQuery(
    string Email,
    string Password
) : IRequest<AuthenticationDto>; 