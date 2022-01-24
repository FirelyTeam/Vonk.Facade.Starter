using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Visi.Repository.Models;
using Vonk.Core.Pluggability;
using Vonk.Core.Pluggability.ContextAware;
using Vonk.Core.Repository;

namespace Visi.Repository
{
    [VonkConfiguration(order: 210)]
    public static class ViSiConfiguration
    {
        public static IServiceCollection AddViSiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ViSiContext>();
            services.TryAddSingleton<ResourceMapper>();
            services.TryAddContextAware<ISearchRepository, ViSiSearchRepository>(ServiceLifetime.Scoped);
            services.TryAddContextAware<IResourceChangeRepository, ViSiChangeRepository>(ServiceLifetime.Scoped);

            services.Configure<DbOptions>(configuration.GetSection(nameof(DbOptions)));
            return services;
        }
    }
}
