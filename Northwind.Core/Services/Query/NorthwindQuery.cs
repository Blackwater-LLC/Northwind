using System.Collections;
using System.Linq.Expressions;

namespace Northwind.Core.Services.Query
{
    /// <summary>
    /// Implements a query for Northwind read operations with LINQ support.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="NorthwindQuery{T}"/> class.
    /// </remarks>
    /// <param name="queryable">The underlying queryable.</param>
    public class NorthwindQuery<T>(IQueryable<T> queryable) : INorthwindQuery<T>
    {
        /// <summary>
        /// Gets the element type of the query.
        /// </summary>
        public Type ElementType => queryable.ElementType;

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="IQueryable"/>.
        /// </summary>
        public Expression Expression => queryable.Expression;

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        public IQueryProvider Provider => queryable.Provider;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<T> GetEnumerator() => queryable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => queryable.GetEnumerator();

        /// <summary>
        /// Asynchronously returns a list of entities.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a list of entities.</returns>
        public Task<List<T>> ToListAsync()
        {
            return Task.Run(() => queryable.ToList());
        }

        /// <summary>
        /// Asynchronously returns the first entity matching the query or default.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the first entity or default.</returns>
        public Task<T> FirstOrDefaultAsync()
        {
            return Task.Run(() => queryable.FirstOrDefault());
        }

        /// <summary>
        /// Applies a filter condition to the query and returns a new query.
        /// </summary>
        /// <param name="predicate">The filter condition.</param>
        /// <returns>An <see cref="INorthwindQuery{T}"/> representing the filtered query.</returns>
        public INorthwindQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            return new NorthwindQuery<T>(queryable.Where(predicate));
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by the selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IQueryable{TResult}"/> whose elements are the result of invoking the transform function on each element of source.</returns>
        public IQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            return queryable.Select(selector);
        }
    }
}
