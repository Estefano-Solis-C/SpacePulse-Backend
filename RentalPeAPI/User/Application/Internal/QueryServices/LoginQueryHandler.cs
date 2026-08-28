using MediatR;
using RentalPeAPI.User.Domain.Repositories;
using RentalPeAPI.User.Domain.Services;

namespace RentalPeAPI.User.Application.Internal.QueryServices;

public class LoginQueryHandler : IRequestHandler<LoginQuery, AuthenticationDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly ITokenGenerationService _tokenGenerationService;

    public LoginQueryHandler(
        IUserRepository userRepository, 
        IPasswordHashingService passwordHashingService, 
        ITokenGenerationService tokenGenerationService)
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
        _tokenGenerationService = tokenGenerationService;
    }

    public async Task<AuthenticationDto> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
       
        var user = await _userRepository.GetByEmailAsync(query.Email);

        if (user is null)
        {
            throw new Exception("Credenciales inválidas."); 
        }

      
        var passwordIsValid = _passwordHashingService.VerifyPassword(query.Password, user.PasswordHash);

        if (!passwordIsValid)
        {
            throw new Exception("Credenciales inválidas.");
        }

       
        var token = _tokenGenerationService.GenerateToken(user);

        
        return new AuthenticationDto(
            user.Id,
            user.FullName,
            user.Email,
            token
        );
    }
}