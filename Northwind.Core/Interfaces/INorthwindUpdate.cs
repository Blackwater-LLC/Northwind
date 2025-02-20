using MongoDB.Driver;
using Northwind.Core.Models.Results;
using System.Linq.Expressions;

namespace Northwind.Core.Interfaces
{
    /// <summary>
    /// Defines methods for updating an entity.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface INorthwindUpdate<T>
    {
        /// <summary>
        /// Asynchronously updates a single entity matching the specified filter using the provided update definition.
        /// </summary>
        /// <param name="filter">The filter expression to match the entity.</param>
        /// <param name="updateDefinition">The update definition specifying the changes.</param>
        /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
        Task<OperationResult<T>> UpdateAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition);
    }
}
