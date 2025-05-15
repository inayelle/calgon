using System.Text.Json;
using System.Text.Json.Serialization;
using Calgon.Host.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Calgon.Host.Mvc;

internal sealed class SignalRModule : IServiceModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        var signalR = services.AddSignalR();

        signalR.AddJsonProtocol(ConfigureJsonProtocol);
    }

    private static void ConfigureJsonProtocol(JsonHubProtocolOptions protocolOptions)
    {
        var serializerOptions = protocolOptions.PayloadSerializerOptions;

        serializerOptions.Converters.Add(new JsonStringEnumConverter());
        serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }
}