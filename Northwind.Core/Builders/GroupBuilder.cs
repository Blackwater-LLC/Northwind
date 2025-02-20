using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Northwind.Core.Models;
using System.Linq.Expressions;

namespace Northwind.Core.Builders
{
    /// <summary>
    /// Provides a fluent builder for configuring a MongoDB group for a given schema type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class GroupBuilder<T>
    {
        /// <summary>
        /// Gets the friendly name of the group.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the MongoDB client instance.
        /// </summary>
        public IMongoClient Client { get; }

        /// <summary>
        /// Gets the MongoDB database instance.
        /// </summary>
        public IMongoDatabase Database { get; }

        /// <summary>
        /// Gets the MongoDB collection associated with the entity type.
        /// </summary>
        public IMongoCollection<T> Collection { get; }

        /// <summary>
        /// Gets the Northwind options.
        /// </summary>
        public NorthwindOptions Options { get; }

        public readonly List<(IndexKeysDefinition<T> Index, bool Unique, string IndexName)> _indexDefinitions =
            [];

        /// <summary>
        /// Gets the read-only list of index definitions.
        /// </summary>
        public IReadOnlyList<(IndexKeysDefinition<T> Index, bool Unique, string IndexName)> IndexDefinitions =>
            _indexDefinitions.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBuilder{T}"/> class.
        /// </summary>
        /// <param name="name">The friendly name of the group.</param>
        /// <param name="client">The MongoDB client instance.</param>
        /// <param name="database">The MongoDB database instance.</param>
        /// <param name="collection">The MongoDB collection for the entity type.</param>
        /// <param name="options">The Northwind options.</param>
        protected GroupBuilder(string name, IMongoClient client, IMongoDatabase database, IMongoCollection<T> collection, NorthwindOptions options)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Group name must not be null or whitespace.", nameof(name));

            Name = name;
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
            Options = options ?? new NorthwindOptions();
        }

        /// <summary>
        /// Adds an index definition for the specified field.
        /// </summary>
        /// <param name="indexExpression">The lambda expression indicating the field to index.</param>
        /// <param name="unique">A value indicating whether the index should enforce uniqueness.</param>
        /// <returns>The current <see cref="GroupBuilder{T}"/> instance.</returns>
        public GroupBuilder<T> WithIndex(Expression<Func<T, object>> indexExpression, bool unique = false)
        {
            ArgumentNullException.ThrowIfNull(indexExpression);

            var indexDefinition = Builders<T>.IndexKeys.Ascending(indexExpression);
            string indexName = GetIndexName(indexExpression);

            _indexDefinitions.Add((indexDefinition, unique, indexName));
            return this;
        }

        /// <summary>
        /// Finalizes the group configuration by creating indexes and registering the group.
        /// </summary>
        public virtual void Build()
        {
            var existingIndexes = new HashSet<string>();
            using (var cursor = Collection.Indexes.List())
            {
                var indexDocs = cursor.ToList();
                foreach (var doc in indexDocs)
                {
                    existingIndexes.Add(doc["name"].AsString);
                }
            }

            var indexModels = new List<CreateIndexModel<T>>();
            foreach (var (index, unique, indexName) in _indexDefinitions)
            {
                if (existingIndexes.Contains(indexName))
                {
                    continue;
                }
                var indexOptions = new CreateIndexOptions { Unique = unique, Name = indexName };
                indexModels.Add(new CreateIndexModel<T>(index, indexOptions));
            }

            if (indexModels.Count > 0)
            {
                Collection.Indexes.CreateMany(indexModels);
            }

            GroupRegistry.Register(this);
        }

        private static string GetIndexName(Expression<Func<T, object>> expression)
        {
            if (expression.Body is MemberExpression member)
            {
                return member.Member.Name + "_1";
            }

            if (expression.Body is UnaryExpression unary && unary.Operand is MemberExpression memberOperand)
            {
                return memberOperand.Member.Name + "_1";
            }

            throw new InvalidOperationException("Invalid index expression");
        }
    }

    /// <summary>
    /// Provides a fluent builder for configuring a MongoDB group for a given schema type with an explicit primary key type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TKey">The primary key type of the entity.</typeparam>
    public class GroupBuilder<T, TKey> : GroupBuilder<T>
    {
        /// <summary>
        /// Gets the primary key expression.
        /// </summary>
        public Expression<Func<T, TKey>> PrimaryKeyExpression { get; private set; } = default!;

        private string? _customKeyFieldName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBuilder{T, TKey}"/> class.
        /// </summary>
        /// <param name="name">The friendly name of the group.</param>
        /// <param name="client">The MongoDB client instance.</param>
        /// <param name="database">The MongoDB database instance.</param>
        /// <param name="collection">The MongoDB collection for the entity type.</param>
        /// <param name="options">The Northwind options.</param>
        internal GroupBuilder(string name, IMongoClient client, IMongoDatabase database, IMongoCollection<T> collection, NorthwindOptions options)
            : base(name, client, database, collection, options)
        {
        }

        /// <summary>
        /// Specifies the primary key property for the entity type.
        /// </summary>
        /// <param name="primaryKeyExpression">The lambda expression indicating the primary key.</param>
        /// <returns>The current <see cref="GroupBuilder{T, TKey}"/> instance.</returns>
        public GroupBuilder<T, TKey> HasPrimaryKey(Expression<Func<T, TKey>> primaryKeyExpression)
        {
            PrimaryKeyExpression = primaryKeyExpression ?? throw new ArgumentNullException(nameof(primaryKeyExpression));
            return this;
        }

        /// <summary>
        /// Specifies a custom field name for the primary key in the MongoDB document.
        /// </summary>
        /// <param name="fieldName">The custom field name to use as the primary key.</param>
        /// <returns>The current <see cref="GroupBuilder{T, TKey}"/> instance.</returns>
        public GroupBuilder<T, TKey> WithCustomKeyFieldName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException("Field name must not be null or whitespace.", nameof(fieldName));
            _customKeyFieldName = fieldName;
            return this;
        }

        /// <summary>
        /// Creates a new instance of <see cref="GroupBuilder{T, TKey}"/>.
        /// </summary>
        /// <param name="name">The friendly name of the group.</param>
        /// <param name="client">The MongoDB client instance.</param>
        /// <param name="database">The MongoDB database instance.</param>
        /// <param name="collection">The MongoDB collection for the entity type.</param>
        /// <param name="options">The Northwind options.</param>
        /// <returns>A new <see cref="GroupBuilder{T, TKey}"/> instance.</returns>
        public static GroupBuilder<T, TKey> New(string name, IMongoClient client, IMongoDatabase database, IMongoCollection<T> collection, NorthwindOptions? options = null)
        {
            return new GroupBuilder<T, TKey>(name, client, database, collection, options ?? new NorthwindOptions());
        }

        /// <summary>
        /// Finalizes the group configuration by creating indexes, registering custom key mappings, and registering the group.
        /// </summary>
        public override void Build()
        {
            if (_customKeyFieldName != null)
            {

                if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
                {
                    BsonClassMap.RegisterClassMap<T>(cm =>
                    {
                        cm.AutoMap();
                        var memberName = GetMemberName(PrimaryKeyExpression);
                        var memberMap = cm.GetMemberMap(memberName);
                        memberMap.SetElementName(_customKeyFieldName);
                    });
                }

                else
                {
                    var cm = BsonClassMap.LookupClassMap(typeof(T));
                    if (!cm.IsFrozen)
                    {
                        var memberName = GetMemberName(PrimaryKeyExpression);
                        var memberMap = cm.GetMemberMap(memberName);
                        memberMap.SetElementName(_customKeyFieldName);
                    }
                }
            }

            base.Build();
        }

        private static string GetMemberName(Expression<Func<T, TKey>> expression)
        {
            if (expression.Body is MemberExpression member)
            {
                return member.Member.Name;
            }

            if (expression.Body is UnaryExpression unary && unary.Operand is MemberExpression memberOperand)
            {
                return memberOperand.Member.Name;
            }

            throw new InvalidOperationException("Invalid primary key expression");
        }
    }
}
