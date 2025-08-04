using EventStore.Azure;
using EventStore.Commands;
using EventStore.EFCore.Postgres;
using EventStore.EFCore.Postgres.Database;
using EventStore.InMemory;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.Testing.SimpleTestDomain;
using Microsoft.EntityFrameworkCore;
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

    public TestConfigurationBuilder WithEFCoreServices()
    {
        _mode = TestMode.EFCore;
        _hostBuilder.AddCoreServices();
        _hostBuilder.AddEFServices(string.Empty);

        _hostBuilder.Services.RemoveAll<DbContextOptions<EventStoreDbContext>>();
        _hostBuilder.Services.AddDbContext<EventStoreDbContext>(options => options.UseInMemoryDatabase("EventStore"));
        
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
        if (_mode == TestMode.EFCore)
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
        if (_mode == TestMode.NotSet)
        {
            _hostBuilder.AddCoreServices();
            _hostBuilder.AddInMemoryServices();
        }

        ServiceHost = _hostBuilder.Build();
    }
}