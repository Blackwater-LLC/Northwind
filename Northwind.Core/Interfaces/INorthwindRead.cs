using Northwind.Core.Models.Results;
using System.Linq.Expressions;

namespace Northwind.Core.Interfaces
{
    /// <summary>
    /// Defines methods for reading an entity.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface INorthwindRead<T>
    {
        /// <summary>
        /// Reads a single entity that matches the specified filter.
        /// </summary>
        /// <param name="filter">The filter expression to match the entity.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing the read entity in its NewData property.</returns>
        OperationResult<T> Read(Expression<Func<T, bool>> filter);

        /// <summary>
        /// Asynchronously reads a single entity that matches the specified filter.
        /// </summary>
        /// <param name="filter">The filter expression to match the entity.</param>
        /// <returns>A task representing the asynchronous operation, containing an <see cref="OperationResult{T}"/> with the read entity in its NewData property.</returns>
        Task<OperationResult<T>> ReadAsync(Expression<Func<T, bool>> filter);
    }
}
