using RentalPeAPI.Property.Domain.Repositories;
using MediatR;

namespace RentalPeAPI.Monitoring.Application.ACL;

/// <summary>
/// Implementación de la Anti-Corruption Layer para Property BC.
/// Proporciona acceso a funcionalidades del contexto de Property sin exponer detalles internos.
/// </summary>
public class PropertyContextFacade : IPropertyContextFacade
{
    private readonly ISpaceRepository _spaceRepository;
    private readonly IMediator _mediator;

    public PropertyContextFacade(ISpaceRepository spaceRepository, IMediator mediator)
    {
        _spaceRepository = spaceRepository;
        _mediator = mediator;
    }

    /// <summary>
    /// Valida que un espacio exista y tenga habilitada la tecnología IoT.
    /// </summary>
    public async Task<bool> ValidateSpaceHasIoTEnabledAsync(long spaceId)
    {
        var space = await _spaceRepository.FindByIdAsync(spaceId);
        return space != null && space.HasIot;
    }

    /// <summary>
    /// Obtiene el estado actual de un espacio como string.
    /// </summary>
    public async Task<string?> GetSpaceStatusAsync(long spaceId)
    {
        var space = await _spaceRepository.FindByIdAsync(spaceId);
        return space?.Status.ToString();
    }

    /// <summary>
    /// Extrae los usuarios (Homeowner y Remodeler) asociados a un espacio.
    /// Se utiliza para despachar notificaciones bifurcadas en eventos del ciclo de vida del proyecto.
    /// </summary>
    public async Task<(Guid OwnerId, Guid? RemodelerId)?> GetSpaceUsersAsync(long spaceId)
    {
        var space = await _spaceRepository.FindByIdAsync(spaceId);
        if (space == null)
            return null;

        return (space.HomeownerId, space.RemodelerId);
    }

    /// <summary>
    /// Obtiene los IDs de espacios asociados a un usuario (como Owner o Remodeler).
    /// Se utiliza en Monitoring para filtrar tareas del usuario.
    /// </summary>
    public async Task<IEnumerable<long>> GetSpaceIdsByUserIdAsync(Guid userId)
    {
        var spaces = await _spaceRepository.ListAsync();
        return spaces
            .Where(s => s.HomeownerId == userId || s.RemodelerId == userId)
            .Select(s => s.Id)
            .ToList();
    }

    /// <summary>
    /// Actualiza el costo total acumulado de las tareas de un espacio.
    /// Despacha un comando interno de Property para evaluar sobrecosto.
    /// </summary>
    public async Task UpdateSpaceTotalPricingAsync(long spaceId, decimal totalPricing)
    {
        await _mediator.Send(
            new RentalPeAPI.Property.Application.Internal.CommandServices.UpdateSpaceTotalPricingCommand(spaceId,
                totalPricing));
    }
}


