using EventStore;
using EventStore.Azure;
using EventStore.Azure.Initialization;
using EventStore.Commands;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.SampleApp.Domain;
using EventStore.SampleApp.Domain.TrafficLights.Commands;
using EventStore.SampleApp.Domain.TrafficLights.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.Services.AddTransient<ProjectionBuilder<TrafficLightProjection>, TrafficLightProjectionBuilder>();
hostBuilder.Services.AddTransient<IProjection, TrafficLightProjection>();

hostBuilder.Services.AddTransient<ICommandHandler<ChangeColour>, ChangeColourCommandHandler>();

hostBuilder.Services.AddHostedService<ChangeColourBackgroundService>();
hostBuilder.Services.AddHostedService<PrintColourBackgroundService>();

hostBuilder.AddCoreServices();
hostBuilder.AddEventBroadcaster();
hostBuilder.AddEventPump();
hostBuilder.AddAzureServices(Defaults.Azure.AzuriteConnectionString);

var host = hostBuilder.Build();

var storage = host.Services.GetService<Storage>();
await storage!.InitializeAsync();

await host.RunAsync();

