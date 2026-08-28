using RentalPeAPI.User.Domain;

namespace RentalPeAPI.User.Domain.Services;

public interface ITokenGenerationService
{
    string GenerateToken(User user);
}