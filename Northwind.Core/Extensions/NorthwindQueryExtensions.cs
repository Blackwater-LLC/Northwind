using System.Linq.Expressions;
using Northwind.Core.Services.Query;

namespace Northwind.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for Northwind queries.
    /// </summary>
    public static class NorthwindQueryExtensions
    {
        /// <summary>
        /// Applies a filter condition to the Northwind query.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="query">The Northwind query instance.</param>
        /// <param name="predicate">The filter condition.</param>
        /// <returns>An <see cref="INorthwindQuery{T}"/> representing the filtered query.</returns>
        public static INorthwindQuery<T> Where<T>(this INorthwindQuery<T> query, Expression<Func<T, bool>> predicate)
        {
            var underlying = ((IQueryable<T>)query).Where(predicate);
            return new NorthwindQuery<T>(underlying);
        }
    }
}
