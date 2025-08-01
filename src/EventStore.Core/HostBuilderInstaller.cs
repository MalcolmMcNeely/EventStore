﻿using EventStore.BackgroundServices;
using EventStore.Commands.Dispatching;
using EventStore.DependencyInjection;
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

        hostBuilder.Services.AddLazy<IEventTypeRegistration, EventTypeRegistration>();
        hostBuilder.Services.AddLazy<IProjectionBuilderRegistration, ProjectionBuilderRegistration>();

        hostBuilder.Services.AddSingleton<EventDispatcher>();
        hostBuilder.Services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
    }
}