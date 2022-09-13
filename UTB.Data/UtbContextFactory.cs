using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTB.Data
{
    internal class UtbContextFactory : IDesignTimeDbContextFactory<UtbContext>
    {
        public UtbContext CreateDbContext(string[] args)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json", optional: true)
               .AddJsonFile($"appsettings.{envName}.json", optional: true)
               .AddEnvFile(".env")
               .Build();

            var builder = new DbContextOptionsBuilder<UtbContext>();

            var connectionString = $"Host={configuration["UTB_PG_HOST"]};Port={configuration["UTB_PG_PORT"]};Database={configuration["UTB_PG_DB"]};Username={configuration["UTB_PG_USER"]};Password={configuration["UTB_PG_PASS"]}";

            builder.UseNpgsql(connectionString, opts =>
            {
                opts.UseNodaTime();
            });
            


            return new UtbContext(builder.Options);
        }
    }
}
