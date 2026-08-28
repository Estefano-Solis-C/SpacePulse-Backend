using System;
using System.Threading.Tasks;

namespace RentalPeAPI.User.Domain.Repositories;

public interface IPaymentMethodRepository
{
    Task AddAsync(PaymentMethod paymentMethod);
}