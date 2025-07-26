using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventStore;

public static class HostApplicationBuilderExtensions
{
    public static void AddDefaultServices(this IHostApplicationBuilder hostBuilder)
    {
        hostBuilder.Services.AddHostedService<EventForwarderBackgroundService>();
    }
}