using System.Reflection;
using EventStore.Azure;
using EventStore.EFCore.Postgres;
using EventStore.EFCore.Postgres.Database;
using EventStore.InMemory;
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

public class TestConfigurationBuilder(string databaseConnectionString = "", string azuriteConnectionString = "")
{
    public HostApplicationBuilder HostBuilder { get; } = Host.CreateApplicationBuilder([]);
    public IHost? ServiceHost { get; private set; }

    TestMode _mode = TestMode.NotSet;
    string DatabaseConnectionString { get; } = databaseConnectionString;
    string AzuriteConnectionString { get; } = azuriteConnectionString;

    public TestConfigurationBuilder WithInMemoryServices()
    {
        _mode = TestMode.InMemory;
        HostBuilder.AddCoreServices();
        HostBuilder.AddInMemoryServices();
        return this;
    }

    public TestConfigurationBuilder WithAzureServices()
    {
        _mode = TestMode.Azure;
        HostBuilder.AddCoreServices();
        HostBuilder.AddAzureServices(AzuriteConnectionString);
        return this;
    }

    public TestConfigurationBuilder WithEFCoreServices(params Assembly[] additionalAssemblies)
    {
        _mode = TestMode.EFCore;
        HostBuilder.AddCoreServices();
        HostBuilder.AddPostgresServices(x =>
        {
            x.ConnectionString = DatabaseConnectionString;
            x.AggregateAssemblies = new[] { typeof(TestingNamespace).Assembly }.Union(additionalAssemblies).ToArray();
        });

        using var scope = HostBuilder.Services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();

        return this;
    }

    public TestConfigurationBuilder With<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface
    {
        HostBuilder.Services.AddSingleton<TInterface, TImplementation>();
        return this;
    }

    public TestConfigurationBuilder Replacing<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface
    {
        var descriptor = new ServiceDescriptor(typeof(TInterface), typeof(TImplementation), ServiceLifetime.Transient);
        HostBuilder.Services.Replace(descriptor);
        return this;
    }

    public void Build()
    {
        if (_mode == TestMode.NotSet)
        {
            HostBuilder.AddCoreServices();
            HostBuilder.AddInMemoryServices();
        }

        ServiceHost = HostBuilder.Build();
    }
}