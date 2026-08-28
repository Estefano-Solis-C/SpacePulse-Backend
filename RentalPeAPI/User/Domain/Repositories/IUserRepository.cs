namespace RentalPeAPI.User.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> UserExistsByEmailAsync(string email);
    Task AddAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
}