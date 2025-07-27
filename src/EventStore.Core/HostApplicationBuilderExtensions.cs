using EventStore.BackgroundServices;
using EventStore.Commands;
using EventStore.Events;
using EventStore.ProjectionBuilders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventStore;

public static class HostApplicationBuilderExtensions
{
    public static void AddCoreServices(this IHostApplicationBuilder hostBuilder)
    {
        hostBuilder.Services.AddHostedService<EventForwarderBackgroundService>();

        hostBuilder.Services.AddSingleton<ProjectionBuilderRegistration>();
        hostBuilder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();
        hostBuilder.Services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
    }
}