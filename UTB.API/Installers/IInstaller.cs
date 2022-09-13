using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UTB.API.Installers
{
    public interface IInstaller
    {
        void Install(IServiceCollection services, IConfiguration configuration);
    }
}