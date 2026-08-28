namespace RentalPeAPI.Property.Application.Internal.QueryServices;

public class ListSpacesQuery
{
    public Guid? OwnerId { get; set; }
    public string? Status { get; set; }

    public ListSpacesQuery(Guid? ownerId = null, string? status = null)
    {
        OwnerId = ownerId;
        Status = status;
    }
}