using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UTB.Console;
using UTB.Data;
using UTB.Data.MappingProfiles;
using UTB.Jobs;

IHostBuilder hostBuilder = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((context, builder) =>
    {
        var path = Path.Combine(Directory.GetCurrentDirectory());
        builder.SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvFile(".env")
            .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddAutoMapper(typeof(DataLayerMappingProfile));

        var configuration = context.Configuration;
        var connectionString = $"Host={configuration["UTB_PG_HOST"]};Port={configuration["UTB_PG_PORT"]};Database={configuration["UTB_PG_DB"]};Username={configuration["UTB_PG_USER"]};Password={configuration["UTB_PG_PASS"]}";

        services.AddUnderTheBayContext(connectionString)
        .AddUnderTheBayServices();

        services.AddDataFetchJobsForWeb();

        services.AddHostedService<DatabaseManagementService>();
    });

var host = hostBuilder.Build();

await host.RunAsync();



