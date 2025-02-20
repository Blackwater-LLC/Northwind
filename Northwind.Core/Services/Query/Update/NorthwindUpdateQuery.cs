using MongoDB.Driver;
using Northwind.Core.Builders;
using Northwind.Core.Models.Results;
using System.Linq.Expressions;

namespace Northwind.Core.Services.Query.Update
{
    /// <summary>
    /// Implements a LINQ-enabled update query for an entity.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class NorthwindUpdateQuery<T> : IUpdateQuery<T>
    {
        private Expression<Func<T, bool>> _filter = x => true;
        private UpdateDefinition<T> _update = Builders<T>.Update.Combine();
        private readonly UpdateDefinitionBuilder<T> _builder = Builders<T>.Update;

        private static Expression<Func<T, bool>> CombineFilters(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// Applies a filter condition to the update query.
        /// </summary>
        /// <param name="predicate">The filter condition.</param>
        /// <returns>The current update query instance.</returns>
        public IUpdateQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            _filter = CombineFilters(_filter, predicate);
            return this;
        }

        /// <summary>
        /// Specifies a field to update with a new value.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field selector expression.</param>
        /// <param name="value">The new value.</param>
        /// <returns>The current update query instance.</returns>
        public IUpdateQuery<T> Set<TField>(Expression<Func<T, TField>> field, TField value)
        {
            _update = _builder.Combine(_update, _builder.Set(field, value));
            return this;
        }

        /// <summary>
        /// Executes the update operation.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
        public async Task<OperationResult<T>> ExecuteAsync()
        {
            var group = GroupRegistry.GetGroup<T>();
            if (group.Options.ReturnDocumentState)
            {
                var updated = await group.Collection.FindOneAndUpdateAsync(_filter, _update, new FindOneAndUpdateOptions<T> { ReturnDocument = ReturnDocument.After });
                if (updated == null)
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
                    Message = "Document updated successfully",
                    State = OperationState.Success,
                    StatusCode = "UPDATE_SUCCESS",
                    NewData = updated
                };
            }
            else
            {
                var updateResult = await group.Collection.UpdateOneAsync(_filter, _update);
                if (updateResult.MatchedCount == 0)
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
                    Message = "Document updated successfully",
                    State = OperationState.Success,
                    StatusCode = "UPDATE_SUCCESS"
                };
            }
        }
    }
}
