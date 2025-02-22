namespace Northwind.Core.Services.Query
{
    /// <summary>
    /// Provides extension methods for LINQ operations on IQueryable.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Asynchronously converts the IQueryable to a List.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="source">The IQueryable source.</param>
        /// <returns>A task representing the asynchronous operation, containing a list of elements.</returns>
        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
        {
            return Task.FromResult(source.ToList());
        }

        /// <summary>
        /// Asynchronously returns the first element of the sequence, or a default value if no element is found.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="source">The IQueryable source.</param>
        /// <returns>A task representing the asynchronous operation, containing the first element or default.</returns>
        public static Task<T?> FirstOrDefaultAsync<T>(this IQueryable<T> source)
        {
            return Task.FromResult(source.FirstOrDefault());
        }
    }
}
