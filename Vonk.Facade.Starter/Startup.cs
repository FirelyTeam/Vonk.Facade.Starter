using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vonk.Core.Configuration;
using Vonk.Fhir.R3;
using Vonk.Core.Pluggability;
using Vonk.Core.Operations.Validation;
using Vonk.Core.Operations.Search;
using Vonk.Core.Repository;
using Vonk.Core.Operations.Crud;

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
                .AddFhirServices()
                .AddVonkMinimalServices()
                .AddSearchServices()
                .AddReadServices()
                .AddRepositorySearchServices()
                .AddViSiServices()
                .AllowResourceTypes("Patient")
                .AddInstanceValidationServices()
                .AddValidationServices() 
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
                .UseSearch()
                .UseRead()
                .UseValidation()
                .UseInstanceValidation()
            ;
        }
    }
}
