using MediatR;
using RentalPeAPI.User.Application.Internal.CommandServices; 

namespace RentalPeAPI.User.Application.Internal.QueryServices;


public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;