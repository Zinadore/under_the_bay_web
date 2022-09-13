using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using UTB.Data.Services;

namespace UTB.Data
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddUnderTheBayContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<UtbContext>(opts =>
            {
                opts.UseNpgsql(connectionString, b =>
                {
                    b.UseNodaTime();
                    b.MigrationsAssembly(typeof(ExtensionMethods).Assembly.GetName().Name);
                });
            });

            return services;
        }

        public static IServiceCollection AddUnderTheBayServices(this IServiceCollection services)
        {
            services.AddTransient<ISamplesService, SamplesService>();
            services.AddTransient<IStationsService, StationsService>();

            return services;
        }

        public static IConfigurationBuilder AddEnvFile(this IConfigurationBuilder builder, string filename)
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), filename);

            // First we check current directory
            if (!File.Exists(filepath))
            {
                // Otherwise we check the parent directory
                filepath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, filename);
            }

            if (File.Exists(filepath))
            {
                foreach (var line in File.ReadLines(filepath))
                {                     var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
                    
                    if (parts.Length != 2)
                        // Malformed line
                        continue;

                    Environment.SetEnvironmentVariable(parts[0], parts[1]);
                }
            }

            return builder.AddEnvironmentVariables();
        }

    }
}