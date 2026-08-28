namespace RentalPeAPI.Property.Application.Internal.QueryServices;

public class GetSpaceByIdQuery
{
    public long Id { get; }

    public GetSpaceByIdQuery(long id)
    {
        Id = id;
    }
}