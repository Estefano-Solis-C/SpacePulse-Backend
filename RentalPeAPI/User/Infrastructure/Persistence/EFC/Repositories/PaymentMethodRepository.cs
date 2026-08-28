using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration;
using RentalPeAPI.User.Domain;
using RentalPeAPI.User.Domain.Repositories;

namespace RentalPeAPI.User.Infrastructure.Persistence.EFC.Repositories;

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly AppDbContext _context;

    public PaymentMethodRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PaymentMethod paymentMethod)
    {
        await _context.Entry(paymentMethod).Reference("User").LoadAsync();
        await _context.PaymentMethods.AddAsync(paymentMethod);
    }
}