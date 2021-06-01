using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Under_the_Bay.API.Jobs;

namespace Under_the_Bay.API.Installers
{
    public class CronSchedulerInstaller: IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScheduler(ctx =>
            {
                ctx.AddJob<DataFetchJob, DataFetchOptions>();
            });
        }
    }
}