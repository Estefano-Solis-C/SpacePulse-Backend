namespace RentalPeAPI.Shared.Domain.Repositories;

/// <summary>
/// Defines the contract for a base repository providing basic CRUD operations for entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity managed by this repository.</typeparam>
public interface IBaseRepository<TEntity>
{
    /// <summary>
    /// Asynchronously adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(TEntity entity);

    /// <summary>
    /// Asynchronously finds an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>A task that represents the asynchronous operation,
    /// containing the entity if found, otherwise null.</returns>
    Task<TEntity?> FindByIdAsync(int id);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Asynchronously retrieves all entities from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, containing an enumerable of all entities.</returns>
    Task<IEnumerable<TEntity>> ListAsync();
}