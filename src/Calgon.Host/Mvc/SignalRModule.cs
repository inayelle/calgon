using System.Text.Json;
using System.Text.Json.Serialization;
using Calgon.Shared;
using Microsoft.AspNetCore.SignalR;

namespace Calgon.Host.Mvc;

internal sealed class SignalRModule : IServiceModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        var signalR = services.AddSignalR();

        signalR.AddJsonProtocol(ConfigureJsonProtocol);
    }

    private static void ConfigureJsonProtocol(JsonHubProtocolOptions jsonHubProtocolOptions)
    {
        var serializerOptions = jsonHubProtocolOptions.PayloadSerializerOptions;

        serializerOptions.Converters.Add(new JsonStringEnumConverter());
        serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }
}
