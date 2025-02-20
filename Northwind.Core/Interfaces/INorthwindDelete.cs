using Northwind.Core.Models.Results;
using System.Linq.Expressions;

namespace Northwind.Core.Interfaces
{
    /// <summary>
    /// Defines methods to delete an entity.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TKey">The primary key type of the entity.</typeparam>
    public interface INorthwindDelete<T, TKey>
    {
        /// <summary>
        /// Deletes an entity based on its primary key.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing the result of the deletion.</returns>
        OperationResult<T> Delete(TKey id);

        /// <summary>
        /// Asynchronously deletes an entity based on its primary key.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>A task representing the asynchronous operation, containing an <see cref="OperationResult{T}"/>.</returns>
        Task<OperationResult<T>> DeleteAsync(TKey id);

        /// <summary>
        /// Deletes an entity based on a condition defined by a field selector and its expected value.
        /// </summary>
        /// <typeparam name="TField">The type of the field used in the condition.</typeparam>
        /// <param name="field">The field selector expression.</param>
        /// <param name="value">The expected value of the field.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing the result of the deletion.</returns>
        OperationResult<T> Delete<TField>(Expression<Func<T, TField>> field, TField value);

        /// <summary>
        /// Asynchronously deletes an entity based on a condition defined by a field selector and its expected value.
        /// </summary>
        /// <typeparam name="TField">The type of the field used in the condition.</typeparam>
        /// <param name="field">The field selector expression.</param>
        /// <param name="value">The expected value of the field.</param>
        /// <returns>A task representing the asynchronous operation, containing an <see cref="OperationResult{T}"/>.</returns>
        Task<OperationResult<T>> DeleteAsync<TField>(Expression<Func<T, TField>> field, TField value);
    }
}
