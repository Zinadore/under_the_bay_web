using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UTB.API.Installers
{
    public class ApiVersioningInstaller: IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }
    }
}