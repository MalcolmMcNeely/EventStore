using EventStore;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.AddDefaultServices();

var host = hostBuilder.Build();
await host.RunAsync();

