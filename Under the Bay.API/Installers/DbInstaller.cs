using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Under_the_Bay.Data;
using Under_the_Bay.Data.Repositories;

namespace Under_the_Bay.API.Installers
{
    public class DbInstaller: IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UtbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PGSQL_DEV"), b =>
                {
                    b.MigrationsAssembly("Under the Bay.API");
                });
            });

            services.AddScoped<IStationsRepository, StationsRepository>();
        }
    }
}