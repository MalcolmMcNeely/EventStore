using EventStore;
using EventStore.Commands;
using EventStore.InMemory;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.SampleApp.InMemory;
using EventStore.SampleApp.InMemory.TrafficLights.Commands;
using EventStore.SampleApp.InMemory.TrafficLights.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.Services.AddTransient<ProjectionBuilder<TrafficLightProjection>, TrafficLightProjectionBuilder>();
hostBuilder.Services.AddTransient<IProjection, TrafficLightProjection>();

hostBuilder.Services.AddTransient<ICommandHandler<ChangeColour>, ChangeColourCommandHandler>();

hostBuilder.Services.AddHostedService<ChangeColourBackgroundService>();
hostBuilder.Services.AddHostedService<PrintColourBackgroundService>();

hostBuilder.AddCoreServices();
hostBuilder.AddInMemoryServices();

var host = hostBuilder.Build();
await host.RunAsync();

