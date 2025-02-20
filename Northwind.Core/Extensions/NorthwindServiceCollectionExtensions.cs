using Microsoft.Extensions.DependencyInjection;
using Northwind.Core.Builders;
using Northwind.Core.Interfaces;
using Northwind.Core.Operations;
using Northwind.Core.Services;

namespace Northwind.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for registering Northwind services.
    /// </summary>
    public static class NorthwindServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the unified Northwind service and all associated operations.
        /// </summary>
        /// <param name="services">The service collection to add Northwind services to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddNorthwind(this IServiceCollection services)
        {
            services.AddSingleton(typeof(INorthwindCreate<>), typeof(NorthwindCreateOperation<>));
            services.AddSingleton(typeof(INorthwindDelete<,>), typeof(NorthwindDeleteOperation<,>));
            services.AddSingleton(typeof(INorthwindRead<>), typeof(NorthwindReadOperation<>));
            services.AddSingleton(typeof(INorthwindUpdate<>), typeof(NorthwindUpdateOperation<>));
            services.AddSingleton(typeof(INorthwindService<>), typeof(NorthwindService<>));

            foreach (var group in GroupRegistry.GetAllGroups())
            {
                services.AddSingleton(group.GetType(), group);
            }

            return services;
        }
    }
}
