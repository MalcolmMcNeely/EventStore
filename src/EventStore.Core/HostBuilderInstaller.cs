using EventStore.BackgroundServices;
using EventStore.Commands;
using EventStore.Events;
using EventStore.ProjectionBuilders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventStore;

public static class HostBuilderInstaller
{
    public static void AddCoreServices(this IHostApplicationBuilder hostBuilder)
    {
        hostBuilder.Services.AddHostedService<EventBroadcasterBackgroundService>();
        hostBuilder.Services.AddHostedService<EventPumpBackgroundService>();

        hostBuilder.Services.AddSingleton<EventTypeRegistration>();
        hostBuilder.Services.AddSingleton<ProjectionBuilderRegistration>();
        hostBuilder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();
        hostBuilder.Services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
    }
}