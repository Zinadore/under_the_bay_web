using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Under_the_Bay.Data;
using Under_the_Bay.Data.Models;
using Under_the_Bay.Data.Repositories;

namespace DataFetcher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
            using IHost host = Host.CreateDefaultBuilder(args).ConfigureServices((ctx, services) =>
                {
                    string connectionString = ctx.Configuration.GetConnectionString("PGSQL_DEV");
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        connectionString = ctx.Configuration["UTB_CONNECTION_STRING"];
                    }

                    services.AddDbContext<UtbContext>(opts =>
                    {
                        opts.UseNpgsql(connectionString);
                    });
                    
                    services.AddTransient<DataFetcherApplication>();
                })
                .UseConsoleLifetime()
                .Build();

            using var scope = host.Services.CreateScope();
            var provider = scope.ServiceProvider;

            var app = provider.GetService<DataFetcherApplication>();

            app?.Run();

            try
            {
                await host.RunAsync();
            }
            catch (Exception e)
            {
                // This throws an Operation Was Canceled exception, as we are calling StopApplication
                // when we are done processing. No clue why.
            }
        }
    }
}