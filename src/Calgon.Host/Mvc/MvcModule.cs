using System.Text.Json;
using System.Text.Json.Serialization;
using Calgon.Host.Extensions;
using Calgon.Host.Mvc.Features;
using Calgon.Host.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Calgon.Host.Mvc;

internal sealed class MvcModule : IServiceModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        var mvc = services.AddControllers(ConfigureMvc);

        mvc
            .AddJsonOptions(ConfigureJsonSerialization)
            .ConfigureApplicationPartManager(ConfigureApplicationPartManager);
    }

    private static void ConfigureMvc(MvcOptions mvcOptions)
    {
        mvcOptions.Filters.Add<NotImplementedExceptionFilter>();
    }

    private static void ConfigureJsonSerialization(JsonOptions jsonOptions)
    {
        var serializerOptions = jsonOptions.JsonSerializerOptions;

        serializerOptions.Converters.Add(new JsonStringEnumConverter());
        serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }

    private static void ConfigureApplicationPartManager(ApplicationPartManager manager)
    {
        manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
    }
}