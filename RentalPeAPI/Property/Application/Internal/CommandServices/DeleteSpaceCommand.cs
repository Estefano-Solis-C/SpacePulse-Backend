namespace RentalPeAPI.Property.Application.Internal.CommandServices
{
    public class DeleteSpaceCommand
    {
        public long Id { get; }

        public DeleteSpaceCommand(long id)
        {
            Id = id;
        }
    }

}