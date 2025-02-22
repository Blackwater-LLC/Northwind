namespace Northwind.Core.Models
{
    /// <summary>
    /// Represents the encryption options for encrypting and decrypting data at rest.
    /// </summary>
    /// <typeparam name="T">The type of the entity to be encrypted or decrypted.</typeparam>
    public class EncryptionOptions<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether encryption should be used.
        /// </summary>
        public bool UseEncryption { get; init; }
        
        /// <summary>
        /// Gets or sets the function used to encrypt a property value.
        /// </summary>
        public Func<object, object> EncryptFunc { get; init; }
        
        /// <summary>
        /// Gets or sets the function used to decrypt a property value.
        /// </summary>
        public Func<object, object> DecryptFunc { get; init; }

        /// <summary>
        /// Encrypts every writable property of the provided entity using the configured encryption function.
        /// </summary>
        /// <param name="entity">The entity to encrypt.</param>
        /// <returns>A new entity with all properties encrypted.</returns>
        public T EncryptEntity(T entity)
        {
            if (!UseEncryption)
                return entity;
            
            var type = typeof(T);
            var encryptedEntity = Activator.CreateInstance<T>();
            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite) continue;
                var originalValue = prop.GetValue(entity);
                
                if (originalValue == null) continue;
                var encryptedValue = EncryptFunc(originalValue);
                
                prop.SetValue(encryptedEntity, encryptedValue);
            }
            return encryptedEntity;
        }

        /// <summary>
        /// Decrypts every writable property of the provided entity using the configured decryption function.
        /// </summary>
        /// <param name="entity">The entity to decrypt.</param>
        /// <returns>A new entity with all properties decrypted.</returns>
        public T DecryptEntity(T entity)
        {
            if (!UseEncryption)
                return entity;
            
            var type = typeof(T);
            var decryptedEntity = Activator.CreateInstance<T>();
            foreach (var prop in type.GetProperties())
            {
                if (prop is not { CanRead: true, CanWrite: true }) continue;
                var encryptedValue = prop.GetValue(entity);
                
                if (encryptedValue == null) continue;
                var decryptedValue = DecryptFunc(encryptedValue);
                
                prop.SetValue(decryptedEntity, decryptedValue);
            }
            return decryptedEntity;
        }
    }
}
