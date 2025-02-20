using MongoDB.Driver;
using Northwind.Core.Models.Results;
using Northwind.Core.Services.Query;
using Northwind.Core.Services.Query.Delete;
using Northwind.Core.Services.Query.Update;
using System.Linq.Expressions;

namespace Northwind.Core.Services
{
    /// <summary>
    /// Implements a unified Northwind service for a specific entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class NorthwindService<T> : INorthwindService<T>
    {
        /// <summary>
        /// Asynchronously creates a new entity.
        /// </summary>
        public Task<OperationResult<T>> CreateAsync(T entity)
        {
            return NorthwindServiceFacade.CreateAsync(entity);
        }

        /// <summary>
        /// Asynchronously deletes an entity based on its primary key.
        /// </summary>
        public Task<OperationResult<T>> DeleteAsync<TField>(TField id)
        {
            return NorthwindServiceFacade.DeleteAsync<T, TField>(id);
        }

        /// <summary>
        /// Asynchronously updates an entity matching the specified filter using the provided update definition.
        /// </summary>
        public Task<OperationResult<T>> UpdateAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition)
        {
            return NorthwindServiceFacade.UpdateAsync(filter, updateDefinition);
        }

        /// <summary>
        /// Asynchronously reads an entity matching the specified filter.
        /// </summary>
        public Task<OperationResult<T>> ReadAsync(Expression<Func<T, bool>> filter)
        {
            return NorthwindServiceFacade.ReadAsync(filter);
        }

        /// <summary>
        /// Returns a LINQ-enabled query for reading entities.
        /// </summary>
        public INorthwindQuery<T> Query()
        {
            return NorthwindServiceFacade.Query<T>();
        }

        /// <summary>
        /// Returns a LINQ-enabled update query for updating entities.
        /// </summary>
        public IUpdateQuery<T> UpdateQuery()
        {
            return NorthwindServiceFacade.UpdateQuery<T>();
        }

        /// <summary>
        /// Returns a LINQ-enabled delete query for deleting entities.
        /// </summary>
        public IDeleteQuery<T> DeleteQuery()
        {
            return NorthwindServiceFacade.DeleteQuery<T>();
        }
    }
}
