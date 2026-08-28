namespace RentalPeAPI.Monitoring.Application.ACL;

/// <summary>
/// Anti-Corruption Layer para acceder al Bounded Context de Property.
/// Permite validar espacios (Spaces) sin exponer detalles internos del contexto.
/// </summary>
public interface IPropertyContextFacade
{
    /// <summary>
    /// Valida que un espacio exista y tenga habilitada la tecnología IoT.
    /// </summary>
    /// <param name="spaceId">ID del espacio a validar</param>
    /// <returns>true si el espacio existe y HasIot es true; false en caso contrario</returns>
    Task<bool> ValidateSpaceHasIoTEnabledAsync(long spaceId);

    /// <summary>
    /// Obtiene el estado actual de un espacio.
    /// </summary>
    /// <param name="spaceId">ID del espacio a consultar</param>
    /// <returns>El estado del espacio como string (ej: "Published", "Accepted", "Finished", "Cancelled"), o null si no existe</returns>
    Task<string?> GetSpaceStatusAsync(long spaceId);

    /// <summary>
    /// Extrae los usuarios (Homeowner y Remodeler) asociados a un espacio.
    /// Se utiliza para despachar notificaciones bifurcadas en eventos del ciclo de vida del proyecto.
    /// </summary>
    /// <param name="spaceId">ID del espacio</param>
    /// <returns>Una tupla con (OwnerId, RemodelerId) si el espacio existe; null en caso contrario</returns>
    Task<(Guid OwnerId, Guid? RemodelerId)?> GetSpaceUsersAsync(long spaceId);

    /// <summary>
    /// Obtiene los IDs de espacios asociados a un usuario (como Owner o Remodeler).
    /// Se utiliza en Monitoring para filtrar tareas del usuario.
    /// </summary>
    /// <param name="userId">ID del usuario (Homeowner o Remodeler)</param>
    /// <returns>Lista de IDs de espacios donde el usuario es Owner o Remodeler</returns>
    Task<IEnumerable<long>> GetSpaceIdsByUserIdAsync(Guid userId);

    /// <summary>
    /// Actualiza el costo total acumulado de las tareas de un espacio y verifica sobrecosto.
    /// Comunica a Property BC el nuevo costo total para que evalúe si se ha excedido el presupuesto.
    /// </summary>
    /// <param name="spaceId">ID del espacio</param>
    /// <param name="totalPricing">Suma total del precio de todas las tareas del espacio</param>
    Task UpdateSpaceTotalPricingAsync(long spaceId, decimal totalPricing);

}

