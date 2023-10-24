namespace DemoRedisCache.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServiceInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            var installers = typeof(Program).Assembly.ExportedTypes
                .Where(w => typeof(IInstaller).IsAssignableFrom(w) && !w.IsInterface && !w.IsAbstract)
                .Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

            foreach (var installer in installers)
            {
                installer.InstallServices(services, configuration);
            }
        }
    }
}
