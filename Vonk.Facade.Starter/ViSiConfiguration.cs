using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return services;
        }
    }
}
