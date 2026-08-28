namespace RentalPeAPI.Property.Application.Internal.CommandServices;

/// <summary>
/// Comando para que un Homeowner cancele un proyecto publicado.
/// Solo se puede cancelar desde estado Published.
/// </summary>
public class CancelSpaceCommand
{
    public long SpaceId { get; set; }
    public Guid RequestingUserId { get; set; }

    public CancelSpaceCommand(long spaceId, Guid requestingUserId)
    {
        if (spaceId <= 0)
            throw new ArgumentException("SpaceId debe ser mayor a 0.", nameof(spaceId));
        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("RequestingUserId no puede estar vacío.", nameof(requestingUserId));

        SpaceId = spaceId;
        RequestingUserId = requestingUserId;
    }
}

