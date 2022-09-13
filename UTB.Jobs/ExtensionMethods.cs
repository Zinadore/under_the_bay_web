using Microsoft.Extensions.DependencyInjection;

using UTB.API.Jobs;

namespace UTB.Jobs
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddDataFetchJobsForWeb(this IServiceCollection services)
        {
            services.AddScheduler(ctx =>
            {
                ctx.AddJob<DataFetchJob, DataFetchOptions>();
            });

            return services;    
        }
    }
}
