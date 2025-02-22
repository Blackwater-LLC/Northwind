namespace Northwind.Core.Services.Query
{
    /// <summary>
    /// Defines a query interface for Northwind read operations with LINQ support.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface INorthwindQuery<T> : IQueryable<T>
    {
        /// <summary>
        /// Asynchronously returns a list of entities.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a list of entities.</returns>
        Task<List<T>> ToListAsync();

        /// <summary>
        /// Asynchronously returns the first entity matching the query or default.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the first entity or default.</returns>
        Task<T?> FirstOrDefaultAsync();
    }
}
