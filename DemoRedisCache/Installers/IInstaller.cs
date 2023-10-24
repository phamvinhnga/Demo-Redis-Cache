using Microsoft.Extensions.DependencyInjection;

namespace DemoRedisCache.Installers
{
    public interface IInstaller
    {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}
