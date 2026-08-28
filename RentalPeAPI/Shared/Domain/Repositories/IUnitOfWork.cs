namespace RentalPeAPI.Shared.Domain.Repositories;

/// <summary>
/// Defines the contract for a unit of work pattern to manage transactions and save changes.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Asynchronously completes the unit of work by saving all changes.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CompleteAsync();
}