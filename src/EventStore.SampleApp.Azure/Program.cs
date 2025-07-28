using EventStore;
using EventStore.Azure;
using EventStore.Azure.Initialization;
using EventStore.Commands;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.SampleApp.Azure;
using EventStore.SampleApp.Azure.TrafficLights.Commands;
using EventStore.SampleApp.Azure.TrafficLights.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.Services.AddTransient<ProjectionBuilder<TrafficLightProjection>, TrafficLightProjectionBuilder>();
hostBuilder.Services.AddTransient<IProjection, TrafficLightProjection>();

hostBuilder.Services.AddTransient<ICommandHandler<ChangeColour>, ChangeColourCommandHandler>();

hostBuilder.Services.AddHostedService<ChangeColourBackgroundService>();
hostBuilder.Services.AddHostedService<PrintColourBackgroundService>();

hostBuilder.AddCoreServices();
hostBuilder.AddAzureServices(AzureContants.AzuriteConnectionString);

var host = hostBuilder.Build();

var storage = host.Services.GetService<Storage>();
await storage.InitializeAsync();

await host.RunAsync();

