namespace RentalPeAPI.Property.Application.Internal.CommandServices;

/// <summary>
/// Comando para que un Remodeler acepte un proyecto (Space) publicado.
/// Solo se puede aceptar desde estado Published y solo por un Remodeler.
/// </summary>
public class AcceptSpaceCommand
{
    public long SpaceId { get; }
    public Guid RemodelerId { get; }

    public AcceptSpaceCommand(long spaceId, Guid remodelerId)
    {
        if (spaceId <= 0)
            throw new ArgumentException("SpaceId debe ser mayor a 0.", nameof(spaceId));
        if (remodelerId == Guid.Empty)
            throw new ArgumentException("RemodelerId no puede estar vacío.", nameof(remodelerId));

        SpaceId = spaceId;
        RemodelerId = remodelerId;
    }
}

