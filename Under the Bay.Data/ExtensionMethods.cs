using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Under_the_Bay.Data
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
                });
            });

            return services;
        }
    }
}