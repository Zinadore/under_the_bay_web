using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Under_the_Bay.API.Installers
{
    public class SwaggerInstaller: IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<SwaggerDefaultValues>();
                c.MapType<DateTimeOffset>(() => new OpenApiSchema { Type = "string", Format = "date"});
                // c.IncludeXmlComments(XmlCommentsFilePath);
            });
        }
    }
}