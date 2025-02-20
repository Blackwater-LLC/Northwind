using MongoDB.Driver;
using Northwind.Core.Builders;
using Northwind.Core.Interfaces;
using Northwind.Core.Models.Results;
using System.Linq.Expressions;

namespace Northwind.Core.Operations
{
    /// <summary>
    /// Implements a type-safe delete operation for entities.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TKey">The primary key type of the entity.</typeparam>
    public class NorthwindDeleteOperation<T, TKey> : INorthwindDelete<T, TKey>
    {
        /// <summary>
        /// Deletes an entity of type T based on its primary key value.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing the result of the deletion.</returns>
        public OperationResult<T> Delete(TKey id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            var group = GroupRegistry.GetGroup<T>();

            if (group is not GroupBuilder<T, TKey> typedGroup)
                throw new InvalidOperationException($"Group primary key type mismatch for {typeof(T).Name}.");

            return Delete(typedGroup.PrimaryKeyExpression, id);
        }

        /// <summary>
        /// Asynchronously deletes an entity of type T based on its primary key value.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>A task representing the asynchronous operation, containing an <see cref="OperationResult{T}"/>.</returns>
        public Task<OperationResult<T>> DeleteAsync(TKey id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            var group = GroupRegistry.GetGroup<T>();

            if (group is not GroupBuilder<T, TKey> typedGroup)
                throw new InvalidOperationException($"Group primary key type mismatch for {typeof(T).Name}.");

            return DeleteAsync(typedGroup.PrimaryKeyExpression, id);
        }

        /// <summary>
        /// Deletes an entity of type T based on a condition defined by a field selector and its expected value.
        /// </summary>
        /// <typeparam name="TField">The type of the field used in the condition.</typeparam>
        /// <param name="field">The field selector expression.</param>
        /// <param name="value">The expected value of the field.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing the result of the deletion.</returns>
        public OperationResult<T> Delete<TField>(Expression<Func<T, TField>> field, TField value)
        {
            ArgumentNullException.ThrowIfNull(field);

            var group = GroupRegistry.GetGroup<T>();
            var filter = Builders<T>.Filter.Eq(field, value);
            var result = group.Collection.DeleteOne(filter);

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

        /// <summary>
        /// Asynchronously deletes an entity of type T based on a condition defined by a field selector and its expected value.
        /// </summary>
        /// <typeparam name="TField">The type of the field used in the condition.</typeparam>
        /// <param name="field">The field selector expression.</param>
        /// <param name="value">The expected value of the field.</param>
        /// <returns>A task representing the asynchronous operation, containing an <see cref="OperationResult{T}"/>.</returns>
        public async Task<OperationResult<T>> DeleteAsync<TField>(Expression<Func<T, TField>> field, TField value)
        {
            ArgumentNullException.ThrowIfNull(field);

            var group = GroupRegistry.GetGroup<T>();
            var filter = Builders<T>.Filter.Eq(field, value);
            var result = await group.Collection.DeleteOneAsync(filter);

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
