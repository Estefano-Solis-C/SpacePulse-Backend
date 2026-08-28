using MediatR;
using System; 
using System.Threading; 
using System.Linq; 
using System.Collections.Generic; 


using RentalPeAPI.User.Domain; 
using RentalPeAPI.User.Application.Internal.CommandServices; 
using RentalPeAPI.User.Domain.Services; 


using SharedIUnitOfWork = RentalPeAPI.Shared.Domain.Repositories.IUnitOfWork;

namespace RentalPeAPI.User.Application.Internal.EventHandlers;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
   
    private readonly RentalPeAPI.User.Domain.Repositories.IUserRepository _userRepository; 
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly SharedIUnitOfWork _unitOfWork; 

    public RegisterUserCommandHandler(
        RentalPeAPI.User.Domain.Repositories.IUserRepository userRepository, 
        IPasswordHashingService passwordHashingService, 
        SharedIUnitOfWork unitOfWork) 
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (await _userRepository.UserExistsByEmailAsync(command.Email))
        {
            throw new Exception("El correo electrónico ya está en uso."); 
        }

        var hashedPassword = _passwordHashingService.HashPassword(command.Password);

        var user = new Domain.User(
            Guid.NewGuid(),
            command.FullName,
            command.Email,
            hashedPassword,
            command.Phone,
            command.Role,
            command.Photo
        );

        await _userRepository.AddAsync(user);
        await _unitOfWork.CompleteAsync();
        
        var paymentMethodsDto = user.PaymentMethods
            .Select(pm => new PaymentMethodDto(pm.Id, pm.Type, pm.LastFourDigits, pm.Expiry))
            .ToList();

        return new UserDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Phone,
            user.CreatedAt,
            user.Role,
            user.Photo,
            paymentMethodsDto
        );
    }
}
