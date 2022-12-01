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

        var Configuration = context.Configuration;
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

        services.AddDataFetchJobsForWeb();

        services.AddHostedService<DatabaseManagementService>();
    });

var host = hostBuilder.Build();

await host.RunAsync();



