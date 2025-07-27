using EventStore.InMemory;
using EventStore.ProjectionBuilders;
using EventStore.SampleApp.InMemory.TrafficLights.Reports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.AddDefaultServices();

hostBuilder.Services.AddTransient<IProjectionBuilder, TrafficLightProjectionBuilder>();
hostBuilder.Services.AddSingleton<ProjectionBuilderRegistration>();
hostBuilder.Services.AddHostedService<MyHostedService>();

var host = hostBuilder.Build();
await host.RunAsync();

public class MyHostedService(ProjectionBuilderRegistration registration) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}