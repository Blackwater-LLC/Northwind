using Northwind.Core.Models.Results;
using System.Linq.Expressions;

namespace Northwind.Core.Services.Query.Update
{
    /// <summary>
    /// Defines a LINQ-enabled update query for an entity.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface IUpdateQuery<T>
    {
        /// <summary>
        /// Applies a filter condition to the update query.
        /// </summary>
        /// <param name="predicate">The filter condition.</param>
        /// <returns>The current update query instance.</returns>
        IUpdateQuery<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Specifies a field to update with a new value.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field selector expression.</param>
        /// <param name="value">The new value.</param>
        /// <returns>The current update query instance.</returns>
        IUpdateQuery<T> Set<TField>(Expression<Func<T, TField>> field, TField value);

        /// <summary>
        /// Executes the update operation.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
        Task<OperationResult<T>> ExecuteAsync();
    }
}
