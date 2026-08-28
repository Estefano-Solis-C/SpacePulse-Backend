
using MediatR;

namespace RentalPeAPI.User.Application.Internal.CommandServices;

public record AddPaymentMethodCommand(
    Guid UserId,
    string Type,
    string Number,
    string Expiry,
    string Cvv
) : IRequest<UserDto>;