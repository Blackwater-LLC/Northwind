using Northwind.Core.Models.Results;
using System.Linq.Expressions;

namespace Northwind.Core.Services.Query.Delete
{
    /// <summary>
    /// Defines a LINQ-enabled delete query for an entity.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface IDeleteQuery<T>
    {
        /// <summary>
        /// Applies a filter condition to the delete query.
        /// </summary>
        /// <param name="predicate">The filter condition.</param>
        /// <returns>The current delete query instance.</returns>
        IDeleteQuery<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Executes the delete operation.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
        Task<OperationResult<T>> ExecuteAsync();
    }
}
