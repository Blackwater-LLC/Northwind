using System.Collections;
using System.Linq.Expressions;
using Northwind.Core.Builders;
using Northwind.Core.Services.Query;

namespace Northwind.Core.Services.Query
{
    /// <summary>
    /// Implements a query for Northwind read operations with LINQ support.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class NorthwindQuery<T> : INorthwindQuery<T>
    {
        private readonly IQueryable<T> _queryable;

        /// <summary>
        /// Initializes a new instance of the <see cref="NorthwindQuery{T}"/> class.
        /// </summary>
        /// <param name="queryable">The underlying queryable.</param>
        public NorthwindQuery(IQueryable<T> queryable)
        {
            _queryable = queryable;
        }

        /// <summary>
        /// Gets the element type of the query.
        /// </summary>
        public Type ElementType => _queryable.ElementType;

        /// <summary>
        /// Gets the expression tree associated with the query.
        /// </summary>
        public Expression Expression => _queryable.Expression;

        /// <summary>
        /// Gets the query provider associated with the query.
        /// </summary>
        public IQueryProvider Provider => _queryable.Provider;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<T> GetEnumerator() => _queryable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _queryable.GetEnumerator();

        /// <summary>
        /// Asynchronously returns a list of entities and decrypts them if encryption is enabled.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a list of entities.</returns>
        public async Task<List<T>> ToListAsync()
        {
            var list = await Task.Run(() => _queryable.ToList());

            var encryptionOptions = GroupRegistry.GetEncryptionOptions<T>();
            if (!encryptionOptions.UseEncryption) return list;
            
            for (var i = 0; i < list.Count; i++)
            {
                list[i] = encryptionOptions.DecryptEntity(list[i]);
            }

            return list;
        }

        /// <summary>
        /// Asynchronously returns the first entity matching the query or default, and decrypts it if encryption is enabled.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the first entity or default.</returns>
        public async Task<T?> FirstOrDefaultAsync()
        {
            var item = await Task.Run(() => _queryable.FirstOrDefault());

            var encryptionOptions = GroupRegistry.GetEncryptionOptions<T>();
            if (encryptionOptions.UseEncryption && item != null)
            {
                item = encryptionOptions.DecryptEntity(item);
            }

            return item;
        }
    }
}
