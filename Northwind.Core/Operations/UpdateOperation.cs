using MongoDB.Driver;
using Northwind.Core.Builders;
using Northwind.Core.Interfaces;
using Northwind.Core.Models.Results;
using System.Linq.Expressions;

namespace Northwind.Core.Operations
{
    /// <summary>
    /// Implements a type-safe update operation for entities.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class NorthwindUpdateOperation<T> : INorthwindUpdate<T>
    {
        /// <summary>
        /// Asynchronously updates a single entity matching the specified filter using the provided update definition.
        /// </summary>
        /// <param name="filter">The filter expression to match the entity.</param>
        /// <param name="updateDefinition">The update definition specifying the changes.</param>
        /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
        public async Task<OperationResult<T>> UpdateAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition)
        {
            ArgumentNullException.ThrowIfNull(filter);
            ArgumentNullException.ThrowIfNull(updateDefinition);

            var group = GroupRegistry.GetGroup<T>();
            OperationResult<T> result;

            if (group.Options.ReturnDocumentState)
            {
                var updated = await group.Collection.FindOneAndUpdateAsync(filter, updateDefinition, new FindOneAndUpdateOptions<T> { ReturnDocument = ReturnDocument.After });
                if (updated == null)
                {
                    result = new OperationResult<T>
                    {
                        Message = "No document found matching the specified condition",
                        State = OperationState.Failure,
                        StatusCode = "NOT_FOUND"
                    };
                }
                else
                {
                    result = new OperationResult<T>
                    {
                        Message = "Document updated successfully",
                        State = OperationState.Success,
                        StatusCode = "UPDATE_SUCCESS",
                        NewData = updated
                    };
                }
            }
            else
            {
                var updateResult = await group.Collection.UpdateOneAsync(filter, updateDefinition);
                if (updateResult.MatchedCount == 0)
                {
                    result = new OperationResult<T>
                    {
                        Message = "No document found matching the specified condition",
                        State = OperationState.Failure,
                        StatusCode = "NOT_FOUND"
                    };
                }
                else
                {
                    result = new OperationResult<T>
                    {
                        Message = "Document updated successfully",
                        State = OperationState.Success,
                        StatusCode = "UPDATE_SUCCESS"
                    };
                }
            }
            return result;
        }
    }
}
