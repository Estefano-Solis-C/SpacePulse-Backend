using Microsoft.EntityFrameworkCore;
using RentalPeAPI.User.Domain;
using RentalPeAPI.User.Domain.Repositories;
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration; 

namespace RentalPeAPI.User.Infrastructure.Persistence.EFC.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UserExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task AddAsync(Domain.User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<Domain.User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.PaymentMethods)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<Domain.User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.PaymentMethods)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}