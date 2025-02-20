using MongoDB.Driver;
using Northwind.Core.Builders;
using Northwind.Core.Models.Results;
using Northwind.Core.Operations;
using Northwind.Core.Services.Query;
using Northwind.Core.Services.Query.Delete;
using Northwind.Core.Services.Query.Update;
using System.Linq.Expressions;

namespace Northwind.Core.Services
{
    /// <summary>
    /// Provides a unified facade for Northwind operations.
    /// </summary>
    public static class NorthwindServiceFacade
    {
        public static Task<OperationResult<T>> CreateAsync<T>(T entity)
        {
            return new NorthwindCreateOperation<T>().CreateAsync(entity);
        }

        public static Task<OperationResult<T>> DeleteAsync<T, TField>(TField id)
        {
            return new NorthwindDeleteOperation<T, TField>().DeleteAsync(id);
        }

        public static Task<OperationResult<T>> UpdateAsync<T>(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition)
        {
            return new NorthwindUpdateOperation<T>().UpdateAsync(filter, updateDefinition);
        }

        public static Task<OperationResult<T>> ReadAsync<T>(Expression<Func<T, bool>> filter)
        {
            return new NorthwindReadOperation<T>().ReadAsync(filter);
        }

        public static INorthwindQuery<T> Query<T>()
        {
            var group = GroupRegistry.GetGroup<T>();
            var queryable = group.Collection.AsQueryable();
            return new NorthwindQuery<T>(queryable);
        }

        public static IUpdateQuery<T> UpdateQuery<T>()
        {
            return new NorthwindUpdateQuery<T>();
        }

        public static IDeleteQuery<T> DeleteQuery<T>()
        {
            return new NorthwindDeleteQuery<T>();
        }
    }
}
