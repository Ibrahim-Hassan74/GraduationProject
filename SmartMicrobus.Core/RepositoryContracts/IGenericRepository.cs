using System.Linq.Expressions;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Gets all entities of type T from the repository.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();
        /// <summary>
        /// Gets all entities of type T from the repository with specified includes for eager loading.
        /// </summary>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        /// <summary>
        /// Gets an entity by its identifier.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<T?> GetByIdAsync(Guid id);
        /// <summary>
        /// Gets an entity by its identifier with specified includes for eager loading.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);
        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity to add.</param>
        Task<T> AddAsync(T entity);
        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity to update.</param>
        Task<T> UpdateAsync(T entity);
        /// <summary>
        /// Deletes an entity from the repository.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="id">The identifier of the entity to delete.</param>
        Task<bool> DeleteAsync(Guid id);
        /// <summary>
        /// count number of row in table
        /// </summary>
        /// <returns>number of row in the table</returns>
        Task<int> CountAsync();
    }
}
