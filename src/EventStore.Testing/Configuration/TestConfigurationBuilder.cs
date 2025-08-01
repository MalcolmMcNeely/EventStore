using EventStore.Azure;
using EventStore.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace EventStore.Testing.Configuration;

public class TestConfigurationBuilder
{
    public IHost? ServiceHost { get; set; }

    readonly HostApplicationBuilder _hostBuilder;

    public TestConfigurationBuilder()
    {
        _hostBuilder = Host.CreateApplicationBuilder([]);
        _hostBuilder.AddCoreServices();
    }
    
    public TestConfigurationBuilder WithInMemoryServices()
    {
        _hostBuilder.AddCoreServices();
        _hostBuilder.AddInMemoryServices();
        return this;
    }
    
    public TestConfigurationBuilder WithAzureServices()
    {
        _hostBuilder.AddCoreServices();
        _hostBuilder.AddAzureServices(Defaults.Azure.AzuriteConnectionString);
        return this;
    }

    public TestConfigurationBuilder With<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface
    {
        _hostBuilder.Services.AddSingleton<TInterface, TImplementation>();
        return this;
    }

    public TestConfigurationBuilder Replacing<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface
    {
        var descriptor = new ServiceDescriptor(typeof(TInterface), typeof(TImplementation), ServiceLifetime.Transient);
        _hostBuilder.Services.Replace(descriptor);
        return this;
    }

    public void Build()
    {
        ServiceHost = _hostBuilder.Build();
    }
}