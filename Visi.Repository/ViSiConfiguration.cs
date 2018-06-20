using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Visi.Repository.Models;
using Vonk.Core.Pluggability;
using Vonk.Core.Repository;

namespace Visi.Repository
{
    [VonkConfiguration(order: 240)]
    public static class ViSiConfiguration
    {
        public static IServiceCollection AddViSiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ViSiContext>();
            services.AddSingleton<ResourceMapper>();
            services.AddScoped<ISearchRepository, ViSiRepository>();

            services.Configure<DbOptions>(configuration.GetSection(nameof(DbOptions)));
            return services;
        }
    }
}
