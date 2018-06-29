using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vonk.Core.Repository;
using Vonk.Facade.Starter.Models;
using Vonk.Facade.Starter.Repository;

namespace Vonk.Facade.Starter
{
    public static class ViSiConfiguration
    {
        public static IServiceCollection AddViSiServices(this IServiceCollection services)
        {
            services.AddDbContext<ViSiContext>();
            services.AddSingleton<ResourceMapper>();
            services.AddScoped<ISearchRepository, ViSiRepository>();
            services.AddScoped<IResourceChangeRepository, ViSiChangeRepository>();

            var sp = services.BuildServiceProvider();
            services.Configure<DbOptions>(sp.GetRequiredService<IConfiguration>().GetSection(nameof(DbOptions)));
            return services;
        }
    }
}
