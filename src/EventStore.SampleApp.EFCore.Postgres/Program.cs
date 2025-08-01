using EventStore;
using EventStore.Commands;
using EventStore.EFCore.Postgres;
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
hostBuilder.AddEFServices();

var host = hostBuilder.Build();
await host.RunAsync();