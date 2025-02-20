using System.Collections.Concurrent;

namespace Northwind.Core.Builders
{
    /// <summary>
    /// Stores group configurations keyed by entity type.
    /// </summary>
    public static class GroupRegistry
    {
        private static readonly ConcurrentDictionary<Type, object> _groups = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Registers the group configuration for the specified entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="groupBuilder">The group builder instance.</param>
        public static void Register<T>(GroupBuilder<T> groupBuilder)
        {
            ArgumentNullException.ThrowIfNull(groupBuilder);
            _groups[typeof(T)] = groupBuilder;
        }

        /// <summary>
        /// Retrieves the registered group configuration for the specified entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The associated group builder.</returns>
        public static GroupBuilder<T> GetGroup<T>()
        {
            if (_groups.TryGetValue(typeof(T), out var group))
            {
                return group as GroupBuilder<T> ?? throw new InvalidOperationException($"Group registered for type {typeof(T).Name} is not of the expected type.");
            }
            throw new InvalidOperationException($"No group registered for type {typeof(T).Name}");
        }

        /// <summary>
        /// Retrieves all registered group configurations.
        /// </summary>
        /// <returns>An enumerable of all registered group configurations.</returns>
        public static IEnumerable<object> GetAllGroups()
        {
            return _groups.Values;
        }
    }
}
