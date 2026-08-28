namespace RentalPeAPI.Property.Application.Internal.CommandServices;

/// <summary>
/// Comando para que un Homeowner complete (finalice) un proyecto.
/// Solo se puede completar desde estado Accepted.
/// </summary>
public class CompleteSpaceCommand
{
    public long SpaceId { get; set; }
    public Guid RequestingUserId { get; set; }

    public CompleteSpaceCommand(long spaceId, Guid requestingUserId)
    {
        if (spaceId <= 0)
            throw new ArgumentException("SpaceId debe ser mayor a 0.", nameof(spaceId));
        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("RequestingUserId no puede estar vacío.", nameof(requestingUserId));

        SpaceId = spaceId;
        RequestingUserId = requestingUserId;
    }
}

