using System.Reflection;
using EventStore.Azure;
using EventStore.Commands;
using EventStore.EFCore.Postgres;
using EventStore.InMemory;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.Testing.SimpleTestDomain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Defaults = EventStore.Azure.Defaults;

namespace EventStore.Testing.Configuration;

public enum TestMode
{
    NotSet,
    InMemory,
    Azure,
    EFCore
}

public class TestConfigurationBuilder
{
    public IHost? ServiceHost { get; set; }
    public TestMode Mode = TestMode.NotSet;
    public string DatabaseConnectionString { get; set; }

    readonly HostApplicationBuilder _hostBuilder;

    public TestConfigurationBuilder()
    {
        _hostBuilder = Host.CreateApplicationBuilder([]);
    }

    public TestConfigurationBuilder WithInMemoryServices()
    {
        Mode = TestMode.InMemory;
        _hostBuilder.AddCoreServices();
        _hostBuilder.AddInMemoryServices();
        return this;
    }

    public TestConfigurationBuilder WithAzureServices()
    {
        Mode = TestMode.Azure;
        _hostBuilder.AddCoreServices();
        _hostBuilder.AddAzureServices(Defaults.Azure.AzuriteConnectionString);
        return this;
    }

    public TestConfigurationBuilder WithEFCoreServices(Assembly withAssembly)
    {
        DatabaseConnectionString = _hostBuilder.Configuration["ConnectionStrings:Postgres"]!;
        Mode = TestMode.EFCore;
        _hostBuilder.AddCoreServices();
        _hostBuilder.AddPostgresServices(DatabaseConnectionString, [typeof(TestingNamespace).Assembly, withAssembly]);
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
        if (Mode == TestMode.EFCore)
        {
            _hostBuilder.Services.AddScoped<ProjectionBuilder<TestProjection>, TestProjectionBuilder>();
            _hostBuilder.Services.AddScoped<IProjection, TestProjection>();
            _hostBuilder.Services.AddScoped<ICommandHandler<TestCommand>, TestCommandHandler>();
        }
        else
        {
            _hostBuilder.Services.AddTransient<ProjectionBuilder<TestProjection>, TestProjectionBuilder>();
            _hostBuilder.Services.AddTransient<IProjection, TestProjection>();
            _hostBuilder.Services.AddTransient<ICommandHandler<TestCommand>, TestCommandHandler>();
        }

        return this;
    }

    public void Build()
    {
        if (Mode == TestMode.NotSet)
        {
            _hostBuilder.AddCoreServices();
            _hostBuilder.AddInMemoryServices();
        }

        ServiceHost = _hostBuilder.Build();
    }
}