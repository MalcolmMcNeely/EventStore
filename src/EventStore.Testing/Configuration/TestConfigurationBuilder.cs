using System.Reflection;
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

namespace EventStore.Testing.Configuration;

public enum TestMode
{
    NotSet,
    InMemory,
    Azure,
    EFCore
}

public class TestConfigurationBuilder(string? databaseConnectionString = null)
{
    public IHost? ServiceHost { get; private set; }
    public TestMode Mode = TestMode.NotSet;
    public string? DatabaseConnectionString { get; set; } = databaseConnectionString;

    readonly HostApplicationBuilder _hostBuilder = Host.CreateApplicationBuilder([]);

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
        _hostBuilder.AddAzureServices(Azure.Defaults.Azure.AzuriteConnectionString);
        return this;
    }

    public TestConfigurationBuilder WithEFCoreServices(params Assembly[] additionalAssemblies)
    {
        //DatabaseConnectionString = _hostBuilder.Configuration["ConnectionStrings:Postgres"]!;
        Mode = TestMode.EFCore;
        _hostBuilder.AddCoreServices();
        _hostBuilder.AddPostgresServices(x =>
        {
            x.ConnectionString = DatabaseConnectionString;
            x.AggregateAssemblies = new[] { typeof(TestingNamespace).Assembly }.Union(additionalAssemblies).ToArray();
        });

        using var scope = _hostBuilder.Services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();

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

    public TestConfigurationBuilder WithTestDomain()
    {
        switch (Mode)
        {
            case TestMode.NotSet:
                throw new TestConfigurationException("Must set test mode before adding test domain");
            case TestMode.EFCore:
                _hostBuilder.Services.AddScoped<ProjectionBuilder<TestProjection>, TestDefaultKeyProjectionBuilder>();
                _hostBuilder.Services.AddScoped<IProjection, TestProjection>();
                _hostBuilder.Services.AddScoped<ICommandHandler<TestCommand>, TestCommandHandler>();
                break;
            default:
                _hostBuilder.Services.AddTransient<ProjectionBuilder<TestProjection>, TestDefaultKeyProjectionBuilder>();
                _hostBuilder.Services.AddTransient<IProjection, TestProjection>();
                _hostBuilder.Services.AddTransient<ICommandHandler<TestCommand>, TestCommandHandler>();
                break;
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