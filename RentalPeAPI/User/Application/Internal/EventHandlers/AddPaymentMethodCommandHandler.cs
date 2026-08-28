using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RentalPeAPI.User.Application.Internal.CommandServices;
using RentalPeAPI.User.Domain;
using RentalPeAPI.User.Domain.Repositories;
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration;
using SharedIUnitOfWork = RentalPeAPI.Shared.Domain.Repositories.IUnitOfWork;

namespace RentalPeAPI.User.Application.Internal.EventHandlers;

public class AddPaymentMethodCommandHandler
    : IRequestHandler<AddPaymentMethodCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly SharedIUnitOfWork _unitOfWork;
    private readonly AppDbContext _context;

    public AddPaymentMethodCommandHandler(
        IUserRepository userRepository,
        SharedIUnitOfWork unitOfWork,
        AppDbContext context)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<UserDto> Handle(AddPaymentMethodCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user is null)
            throw new Exception("Usuario no encontrado.");
        if (string.IsNullOrWhiteSpace(command.Number))
            throw new ArgumentException("Number is required.", nameof(command.Number));
        if (string.IsNullOrWhiteSpace(command.Type))
            throw new ArgumentException("Type is required.", nameof(command.Type));
        if (string.IsNullOrWhiteSpace(command.Expiry))
            throw new ArgumentException("Expiry is required.", nameof(command.Expiry));
        string lastFour = command.Number.Length >= 4 
            ? command.Number.Substring(command.Number.Length - 4) 
            : command.Number;
        var paymentMethod = new PaymentMethod(Guid.NewGuid(), command.UserId, command.Type, lastFour, command.Expiry);
        await _context.PaymentMethods.AddAsync(paymentMethod);
        await _unitOfWork.CompleteAsync();
        user.PaymentMethods.Add(paymentMethod);
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
