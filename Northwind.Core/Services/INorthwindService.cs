using MongoDB.Driver;
using Northwind.Core.Models.Results;
using Northwind.Core.Services.Query;
using Northwind.Core.Services.Query.Delete;
using Northwind.Core.Services.Query.Update;
using System.Linq.Expressions;

namespace Northwind.Core.Services
{
    /// <summary>
    /// Defines a unified Northwind service for a specific entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface INorthwindService<T>
    {
        /// <summary>
        /// Asynchronously creates a new entity.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
        Task<OperationResult<T>> CreateAsync(T entity);

        /// <summary>
        /// Asynchronously deletes an entity based on its primary key.
        /// </summary>
        /// <typeparam name="TField">The type of the primary key.</typeparam>
        /// <param name="id">The primary key value.</param>
        /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
        Task<OperationResult<T>> DeleteAsync<TField>(TField id);

        /// <summary>
        /// Asynchronously updates an entity matching the specified filter using the provided update definition.
        /// </summary>
        /// <param name="filter">The filter expression to match the entity.</param>
        /// <param name="updateDefinition">The update definition specifying the changes.</param>
        /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
        Task<OperationResult<T>> UpdateAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition);

        /// <summary>
        /// Asynchronously reads an entity matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter expression to match the entity.</param>
        /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
        Task<OperationResult<T>> ReadAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        /// Returns a LINQ-enabled query for reading entities.
        /// </summary>
        /// <returns>An <see cref="INorthwindQuery{T}"/> instance.</returns>
        INorthwindQuery<T> Query();

        /// <summary>
        /// Returns a LINQ-enabled update query for updating entities.
        /// </summary>
        /// <returns>An <see cref="INorthwindQuery{T}"/> instance.</returns>
        IUpdateQuery<T> UpdateQuery();

        /// <summary>
        /// Returns a LINQ-enabled delete query for deleting entities.
        /// </summary>
        /// <returns>An <see cref="IDeleteQuery{T}"/> instance.</returns>
        IDeleteQuery<T> DeleteQuery();
    }
}
