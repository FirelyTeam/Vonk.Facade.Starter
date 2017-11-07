using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Vonk.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Vonk.Model.FromApi;
using Vonk.Core.Context;
using Vonk.Core.Operations.Capability;
using Vonk.Core.Licensing;
using Vonk.Core.Pluggability;

namespace Vonk.Facade.Starter
{
    public class Startup
    {
        private readonly IConfigurationRoot _configurationRoot;

        public Startup(IHostingEnvironment env)
        {
            _configurationRoot = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(path: "appsettings.json", reloadOnChange: true, optional: true)
                .Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_configurationRoot)
                .AddApiContextServices()
                .AddVonkMinimalServices()
                .AllowResourceTypes("Patient")
            ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseVonkMinimal()
            ;
        }
    }
}
