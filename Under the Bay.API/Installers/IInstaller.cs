using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Under_the_Bay.API.Installers
{
    public interface IInstaller
    {
        void Install(IServiceCollection services, IConfiguration configuration);
    }
}