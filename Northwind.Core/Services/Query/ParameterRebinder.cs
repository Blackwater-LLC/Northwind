using System.Linq.Expressions;

namespace Northwind.Core.Services.Query
{
    /// <summary>
    /// Replaces parameters in an expression.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ParameterRebinder"/> class.
    /// </remarks>
    /// <param name="map">The map of parameters to replace.</param>
    public class ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map) : ExpressionVisitor
    {
        /// <summary>
        /// Replaces the parameters in the specified expression.
        /// </summary>
        /// <param name="map">The map of parameters to replace.</param>
        /// <param name="exp">The expression.</param>
        /// <returns>The expression with replaced parameters.</returns>
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        /// <inheritdoc/>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (map.TryGetValue(node, out ParameterExpression replacement))
            {
                return replacement;
            }
            return base.VisitParameter(node);
        }
    }
}
