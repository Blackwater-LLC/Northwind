using MongoDB.Driver;
using Northwind.Core.Builders;
using Northwind.Core.Interfaces;
using Northwind.Core.Models.Results;
using System.Reflection;
using Northwind.Core.Models;

namespace Northwind.Core.Operations
{
    /// <summary>
    /// Implements a type-safe create operation for entities.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class NorthwindCreateOperation<T> : INorthwindCreate<T>
    {
        private readonly EncryptionOptions<T> _encryptionOptions = GroupRegistry.GetEncryptionOptions<T>();
        
        /// <summary>
        /// Creates a new entity of type T in its configured MongoDB collection.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing the result of the creation.</returns>
        public OperationResult<T> Create(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            var group = GroupRegistry.GetGroup<T>();

            foreach (var (_, unique, indexName) in group.IndexDefinitions)
            {
                if (!unique) continue;
                var fieldName = indexName.EndsWith("_1") ? indexName[..^2] : indexName;
                
                var property = typeof(T).GetProperty(fieldName);
                if (property == null) continue;
                
                var value = property.GetValue(entity);
                var filter = Builders<T>.Filter.Eq(fieldName, value);
                var exists = group.Collection.Find(filter).Any();

                if (exists)
                {
                    return new OperationResult<T>
                    {
                        Message = $"Creation violates the unique index definition for field '{fieldName}'",
                        State = OperationState.Failure,
                        StatusCode = "UNIQUE_INDEX_VIOLATION"
                    };
                }
            }

            try
            {
                if (_encryptionOptions.UseEncryption)
                    entity = _encryptionOptions.EncryptEntity(entity);
                
                if (group.Options.UseTransactions)
                {
                    using var session = group.Client.StartSession();
                    session.StartTransaction();
                    
                    group.Collection.InsertOne(session, entity);
                    session.CommitTransaction();
                }
                else
                {
                    group.Collection.InsertOne(entity);
                }
                return new OperationResult<T>
                {
                    Message = "Document created successfully",
                    State = OperationState.Success,
                    StatusCode = "CREATION_SUCCESS",
                    NewData = group.Options.ReturnDocumentState ? entity : default
                };
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Code == 11000)
                {
                    return new OperationResult<T>
                    {
                        Message = "Duplicate key error",
                        State = OperationState.Failure,
                        StatusCode = "DUPLICATE_KEY"
                    };
                }
                throw;
            }
        }

        /// <summary>
        /// Asynchronously creates a new entity of type T in its configured MongoDB collection.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <returns>A task representing the asynchronous operation, containing an <see cref="OperationResult{T}"/>.</returns>
        public async Task<OperationResult<T>> CreateAsync(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            var group = GroupRegistry.GetGroup<T>();

            foreach (var (_, unique, indexName) in group.IndexDefinitions)
            {
                if (!unique) continue;
                
                var fieldName = indexName.EndsWith("_1") ? indexName[..^2] : indexName;
                var property = typeof(T).GetProperty(fieldName);
                
                if (property == null) continue;
                var value = property.GetValue(entity);
                
                var filter = Builders<T>.Filter.Eq(fieldName, value);
                var exists = await group.Collection.Find(filter).AnyAsync();
                
                if (exists)
                {
                    return new OperationResult<T>
                    {
                        Message = $"Creation violates the unique index definition for field '{fieldName}'",
                        State = OperationState.Failure,
                        StatusCode = "UNIQUE_INDEX_VIOLATION"
                    };
                }
            }

            try
            {
                if (_encryptionOptions.UseEncryption)
                   entity = _encryptionOptions.EncryptEntity(entity);
                
                if (group.Options.UseTransactions)
                {
                    using var session = await group.Client.StartSessionAsync();
                    session.StartTransaction();
                    await group.Collection.InsertOneAsync(session, entity);
                    await session.CommitTransactionAsync();
                }
                else
                {
                    await group.Collection.InsertOneAsync(entity);
                }
                return new OperationResult<T>
                {
                    Message = "Document created successfully",
                    State = OperationState.Success,
                    StatusCode = "CREATION_SUCCESS",
                    NewData = group.Options.ReturnDocumentState ? entity : default
                };
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Code == 11000)
                {
                    return new OperationResult<T>
                    {
                        Message = "Duplicate key error",
                        State = OperationState.Failure,
                        StatusCode = "DUPLICATE_KEY"
                    };
                }
                throw;
            }
        }
    }
}
