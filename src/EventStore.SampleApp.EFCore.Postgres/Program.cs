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
var databaseConnectionString = hostBuilder.Configuration["ConnectionStrings:Postgres"]!;

hostBuilder.Services.AddScoped<ProjectionBuilder<TrafficLightProjection>, TrafficLightProjectionBuilder>();
hostBuilder.Services.AddScoped<IProjection, TrafficLightProjection>();

hostBuilder.Services.AddScoped<ICommandHandler<ChangeColour>, ChangeColourCommandHandler>();

hostBuilder.Services.AddHostedService<ChangeColourBackgroundService>();
hostBuilder.Services.AddHostedService<PrintColourBackgroundService>();

hostBuilder.AddCoreServices();
hostBuilder.AddEventBroadcaster();
hostBuilder.AddEventPump();
hostBuilder.AddPostgresServices(databaseConnectionString, typeof(AppDomainNamespace).Assembly);

var host = hostBuilder.Build();
await host.RunAsync();