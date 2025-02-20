using Northwind.Core.Builders;
using Northwind.Core.Models.Results;
using System.Linq.Expressions;

namespace Northwind.Core.Services.Query.Delete
{
    /// <summary>
    /// Implements a LINQ-enabled delete query for an entity.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class NorthwindDeleteQuery<T> : IDeleteQuery<T>
    {
        private Expression<Func<T, bool>> _filter = x => true;

        private static Expression<Func<T, bool>> CombineFilters(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// Applies a filter condition to the delete query.
        /// </summary>
        /// <param name="predicate">The filter condition.</param>
        /// <returns>The current delete query instance.</returns>
        public IDeleteQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            _filter = CombineFilters(_filter, predicate);
            return this;
        }

        /// <summary>
        /// Executes the delete operation.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
        public async Task<OperationResult<T>> ExecuteAsync()
        {
            var group = GroupRegistry.GetGroup<T>();
            var result = await group.Collection.DeleteOneAsync(_filter);
            if (result.DeletedCount == 0)
            {
                return new OperationResult<T>
                {
                    Message = "No document found matching the specified condition",
                    State = OperationState.Failure,
                    StatusCode = "NOT_FOUND"
                };
            }
            return new OperationResult<T>
            {
                Message = "Document deleted successfully",
                State = OperationState.Success,
                StatusCode = "DELETION_SUCCESS"
            };
        }
    }
}
