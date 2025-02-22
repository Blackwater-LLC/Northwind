using System.Linq.Expressions;
using MongoDB.Driver;
using Northwind.Core.Builders;
using Northwind.Core.Models;
using Northwind.Core.Models.Results;

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
        private readonly EncryptionOptions<T> _encryptionOptions = GroupRegistry.GetEncryptionOptions<T>();

        private static Expression<Func<T, bool>> CombineFilters(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));
            var body = Expression.AndAlso(
                Expression.Invoke(first, parameter),
                Expression.Invoke(second, parameter)
            );
            return Expression.Lambda<Func<T, bool>>(body, parameter);
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
        /// Specifies a field to update with a new value. The value is encrypted if encryption is enabled.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field selector expression.</param>
        /// <param name="value">The new value.</param>
        /// <returns>The current update query instance.</returns>
        public IUpdateQuery<T> Set<TField>(Expression<Func<T, TField>> field, TField value)
        {
            var encryptedValue = value;
            
            if (_encryptionOptions.UseEncryption)
            {
                if (value != null)
                {
                    var result = _encryptionOptions.EncryptFunc(value);
                    encryptedValue = (TField)result;
                }
            }

            _update = _builder.Combine(_update, _builder.Set(field, encryptedValue));
            return this;
        }

        /// <summary>
        /// Executes the update operation and decrypts the updated document if encryption is enabled.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the operation result.</returns>
        public async Task<OperationResult<T>> ExecuteAsync()
        {
            var group = GroupRegistry.GetGroup<T>();

            if (group.Options.ReturnDocumentState)
            {
                var updated = await group.Collection.FindOneAndUpdateAsync(
                    _filter,
                    _update,
                    new FindOneAndUpdateOptions<T> { ReturnDocument = ReturnDocument.After }
                );

                if (updated != null && _encryptionOptions is { UseEncryption: true, DecryptFunc: not null })
                {
                    updated = _encryptionOptions.DecryptEntity(updated);
                }

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
