using Microsoft.Extensions.DependencyInjection;

namespace Calgon.Shared;

public interface IServiceModule
{
    void ConfigureServices(IServiceCollection services);
}

public static class ServiceModuleExtensions
{
    public static IServiceCollection AddModule<TModule>(this IServiceCollection services)
        where TModule : IServiceModule, new()
    {
        var module = new TModule();

        module.ConfigureServices(services);

        return services;
    }
}