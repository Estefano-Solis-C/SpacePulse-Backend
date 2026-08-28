namespace RentalPeAPI.Property.Application.Internal.QueryServices;

/// <summary>
/// Query para obtener todos los espacios asociados a un usuario.
/// Retorna espacios donde el usuario es propietario (HomeownerId) o remodelador asignado (RemodelerId).
/// </summary>
public class GetSpacesByUserIdQuery
{
    public Guid UserId { get; set; }

    public GetSpacesByUserIdQuery(Guid userId)
    {
        UserId = userId;
    }
}

