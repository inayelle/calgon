namespace Calgon.Host.Extensions;

internal interface IServiceModule
{
    void ConfigureServices(IServiceCollection services);
}

internal static class ServiceModuleExtensions
{
    public static IServiceCollection AddModule<TModule>(this IServiceCollection services)
        where TModule : IServiceModule, new()
    {
        var module = new TModule();

        module.ConfigureServices(services);

        return services;
    }
}