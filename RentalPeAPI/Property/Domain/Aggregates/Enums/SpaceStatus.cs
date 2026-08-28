namespace RentalPeAPI.Property.Domain.Aggregates.Enums;

/// <summary>
/// Estados de un espacio en el ciclo de vida de una remodelación.
/// </summary>
public enum SpaceStatus
{
    Published = 0,
    Accepted = 1,
    InProgress = 2,
    Finished = 3,
    Cancelled = 4
}

