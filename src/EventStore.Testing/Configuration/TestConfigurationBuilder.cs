using EventStore.Azure;
using EventStore.Commands;
using EventStore.InMemory;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.Testing.BasicTestCase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace EventStore.Testing.Configuration;

public enum TestMode
{
    NotSet,
    InMemory,
    Azure
}

public class TestConfigurationBuilder
{
    public IHost? ServiceHost { get; set; }

    readonly HostApplicationBuilder _hostBuilder;
    
    TestMode _mode = TestMode.NotSet;

    public TestConfigurationBuilder()
    {
        _hostBuilder = Host.CreateApplicationBuilder([]);
        _hostBuilder.AddCoreServices();
    }
    
    public TestConfigurationBuilder WithInMemoryServices()
    {
        _mode = TestMode.InMemory;
        _hostBuilder.AddCoreServices();
        _hostBuilder.AddInMemoryServices();
        return this;
    }
    
    public TestConfigurationBuilder WithAzureServices()
    {
        _mode = TestMode.Azure;
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

    public TestConfigurationBuilder WithBasicTestCase()
    {
        _hostBuilder.Services.AddTransient<ProjectionBuilder<TestProjection>, TestProjectionBuilder>();
        _hostBuilder.Services.AddTransient<IProjection, TestProjection>();
        _hostBuilder.Services.AddTransient<ICommandHandler<TestCommand>, TestCommandHandler>();
        return this;
    }

    public void Build() 
    {
        if (_mode == TestMode.NotSet)
        {
            _hostBuilder.AddCoreServices();
            _hostBuilder.AddInMemoryServices();
        }

        ServiceHost = _hostBuilder.Build();
    }
}