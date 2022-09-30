using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UTB.Data;
using Microsoft.EntityFrameworkCore;

namespace UTB.Console
{
    internal sealed class DatabaseManagementService : IHostedService
    {
        private readonly ILogger<DatabaseManagementService> logger;
        private readonly IHostApplicationLifetime lifetime;
        private readonly UtbContext context;

        public DatabaseManagementService(ILogger<DatabaseManagementService> logger, IHostApplicationLifetime lifetime, UtbContext context)
        {
            this.logger = logger;
            this.lifetime = lifetime;
            this.context = context;
            lifetime.ApplicationStarted.Register(OnStarted);
            lifetime.ApplicationStopping.Register(OnStopping);
            lifetime.ApplicationStopped.Register(OnStopped);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var databaseReady = false;
            var seconds = 15;

            while (!databaseReady)
            {
                databaseReady = await context.CheckIfDatabaseReady();

                if (databaseReady) {
                    var migrations = await context.Database.GetPendingMigrationsAsync();

                    if (migrations.Count() > 0) {
                        logger.LogInformation("Database appears ready, proceed to migrate");
                        await context.Database.MigrateAsync();
                    }
                }
                else {
                    logger.LogInformation($"Database is not ready, waiting {seconds} seconds...");
                    await Task.Delay(seconds * 1000);
                    seconds *= 2;
                }
            }
        }

        private void OnStarted()
        {
        }

        private void OnStopping()
        {
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStopped()
        {
        }

    }
}
