using System;
using System.Linq;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UTB.Data;
using UTB.API.Installers;
using UTB.Data.MappingProfiles;
using UTB.Jobs;

namespace UTB.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // services.AddControllers()
            //     .AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddControllers();
            
            services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

            var installers = typeof(Startup).Assembly.ExportedTypes
                .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance).ToList();
            
            foreach (IInstaller installer in installers)
            {
                installer.Install(services, Configuration);
            }
            
            services.AddAutoMapper(typeof(Startup), typeof(DataLayerMappingProfile));


            var connectionString = @$"
            Host={Configuration["UTB_PG_HOST"]};
            Port={Configuration["UTB_PG_PORT"]};
            Database={Configuration["UTB_PG_DB"]};
            Username={Configuration["UTB_PG_USER"]};
            Password={Configuration["UTB_PG_PASS"]};
            Timeout=300;
            Keepalive=300;
            CommandTimeout=300";

            services.AddUnderTheBayContext(connectionString)
                .AddUnderTheBayServices();

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider  provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Under the Bay - Swagger UI";
                
                foreach(var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    c.RoutePrefix = string.Empty;
                }
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
