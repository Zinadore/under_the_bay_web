using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTB.Console
{
    internal sealed class ExampleHostingService : IHostedService
    {
        private readonly ILogger<ExampleHostingService> logger;
        private readonly IHostApplicationLifetime lifetime;

        public ExampleHostingService(ILogger<ExampleHostingService> logger, IHostApplicationLifetime lifetime)
        {
            this.logger = logger;
            this.lifetime = lifetime;

            lifetime.ApplicationStarted.Register(OnStarted);
            lifetime.ApplicationStopping.Register(OnStopping);
            lifetime.ApplicationStopped.Register(OnStopped);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("1. StartAsync has been called.");

            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            logger.LogInformation("2. OnStarted has been called.");
        }

        private void OnStopping()
        {
            logger.LogInformation("3. OnStopping has been called.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("4. StopAsync has been called.");

            return Task.CompletedTask;
        }

        private void OnStopped()
        {
            logger.LogInformation("5. OnStopped has been called.");
        }

    }
}
