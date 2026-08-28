using RentalPeAPI.User.Domain.Services;


using BCryptNet = BCrypt.Net.BCrypt; 

namespace RentalPeAPI.User.Infrastructure.Security;

public class PasswordHashingService : IPasswordHashingService
{
    public string HashPassword(string password)
    {
        return BCryptNet.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCryptNet.Verify(password, hash);
    }
}