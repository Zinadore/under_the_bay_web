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

                    services.AddUnderTheBayContext(connectionString);
                    
                    services.AddTransient<DataFetcherApplication>();
                })
                .UseConsoleLifetime()
                .Build();

            using var scope = host.Services.CreateScope();
            var provider = scope.ServiceProvider;

            var app = provider.GetService<DataFetcherApplication>();

            if (app == null)
                return;
            try
            {
                await app.RunAsync();
                await host.RunAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            
            return;
        }
    }
}