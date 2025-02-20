using Northwind.Core.Models.Results;

namespace Northwind.Core.Interfaces
{
    /// <summary>
    /// Defines a method to create an entity.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface INorthwindCreate<T>
    {
        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing the result of the creation.</returns>
        OperationResult<T> Create(T entity);

        /// <summary>
        /// Asynchronously creates a new entity.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <returns>A task representing the asynchronous operation, containing an <see cref="OperationResult{T}"/>.</returns>
        Task<OperationResult<T>> CreateAsync(T entity);
    }
}
