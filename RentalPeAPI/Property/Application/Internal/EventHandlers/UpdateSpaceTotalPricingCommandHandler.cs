using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RentalPeAPI.Property.Application.Internal.CommandServices;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Property.Application.Internal.EventHandlers;

/// <summary>
/// Manejador del comando UpdateSpaceTotalPricingCommand.
/// Evalúa si el costo total de las tareas de un espacio excede el presupuesto estimado.
/// Si hay sobrecosto y no ha sido notificado al propietario, despacha una alerta.
/// 
/// MOTOR DE ALERTA DE SOBRECOSTO:
/// 1. Actualiza el costo total acumulado (EndingPricing)
/// 2. Compara con el presupuesto estimado (EstimatedBudget)
/// 3. Si (EndingPricing > EstimatedBudget) y no notificado: marca como notificado y envía alerta
/// 4. Persite cambios en la base de datos
/// </summary>
public class UpdateSpaceTotalPricingCommandHandler : IRequestHandler<UpdateSpaceTotalPricingCommand, bool>
{
    private readonly ISpaceRepository _spaceRepository;
    private readonly IMonitoringContextFacade _monitoringFacade;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSpaceTotalPricingCommandHandler(
        ISpaceRepository spaceRepository,
        IMonitoringContextFacade monitoringFacade,
        IUnitOfWork unitOfWork)
    {
        _spaceRepository = spaceRepository;
        _monitoringFacade = monitoringFacade;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSpaceTotalPricingCommand request, CancellationToken cancellationToken)
    {
        var space = await _spaceRepository.FindByIdAsync(request.SpaceId) 
            ?? throw new KeyNotFoundException($"Space con ID {request.SpaceId} no encontrado.");
        
        space.UpdateEndingPricing(request.TotalPricing);
        
        bool isOverBudget = space.EndingPricing > space.EstimatedBudget;
        
        if (isOverBudget && !space.IsOverBudgetNotified)
        {
            space.MarkAsOverBudgetNotified();
            
            var overBudgetAmount = space.EndingPricing - space.EstimatedBudget;
            string alertMessage = 
                $"El costo total de las tareas ({space.Currency} {space.EndingPricing:F2}) " +
                $"ha superado tu presupuesto estimado inicial ({space.Currency} {space.EstimatedBudget:F2}). " +
                $"Contacta al remodelador si tienes inquietudes sobre los costos.";
            
            await _monitoringFacade.DispatchNotificationAsync(
                space.HomeownerId,
                space.Id,
                "Presupuesto Excedido",
                alertMessage
            );
        }
        
        await _unitOfWork.CompleteAsync();

        return true;
    }
}

