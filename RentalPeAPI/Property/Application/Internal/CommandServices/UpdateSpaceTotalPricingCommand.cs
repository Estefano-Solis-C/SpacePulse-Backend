using MediatR;

namespace RentalPeAPI.Property.Application.Internal.CommandServices;

/// <summary>
/// Comando interno para actualizar el costo total acumulado de las tareas de un espacio.
/// Este comando es despachado por Monitoring BC a través de la fachada ACL (PropertyContextFacade)
/// cuando el remodelador actualiza el precio de una tarea.
/// 
/// RESPONSABILIDAD: Evaluar si el costo total ha excedido el presupuesto estimado
/// y despachar una notificación de alerta al Homeowner si es necesario.
/// </summary>
public record UpdateSpaceTotalPricingCommand(
    long SpaceId,
    decimal TotalPricing
) : IRequest<bool>;

