using MongoDB.Driver;
using Northwind.Core.Builders;
using Northwind.Core.Interfaces;
using Northwind.Core.Models.Results;
using System.Linq.Expressions;
using Northwind.Core.Models;

namespace Northwind.Core.Operations
{
    /// <summary>
    /// Implements a type-safe read operation for entities.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class NorthwindReadOperation<T> : INorthwindRead<T>
    {
        private readonly EncryptionOptions<T> _encryptionOptions = GroupRegistry.GetEncryptionOptions<T>();
        public OperationResult<T> Read(Expression<Func<T, bool>> filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            var group = GroupRegistry.GetGroup<T>();
            var entity = group.Collection.Find(filter).FirstOrDefault();

            if (_encryptionOptions.UseEncryption)
                entity = _encryptionOptions.DecryptEntity(entity);

            if (entity == null)
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
                Message = "Document read successfully",
                State = OperationState.Success,
                StatusCode = "READ_SUCCESS",
                NewData = entity
            };
        }

        public async Task<OperationResult<T>> ReadAsync(Expression<Func<T, bool>> filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            var group = GroupRegistry.GetGroup<T>();
            var entity = await group.Collection.Find(filter).FirstOrDefaultAsync();
            
            if (_encryptionOptions.UseEncryption)
                entity = _encryptionOptions.DecryptEntity(entity);

            if (entity == null)
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
                Message = "Document read successfully",
                State = OperationState.Success,
                StatusCode = "READ_SUCCESS",
                NewData = entity
            };
        }
    }
}
