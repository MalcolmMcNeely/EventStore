using DotNet.Testcontainers.Builders;
using EventStore.Azure.Azure;
using EventStore.Azure.Initialization;
using EventStore.Testing.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Azurite;

namespace EventStore.Azure.Tests;

[SetUpFixture]
public class AzuriteTestFixture
{
    readonly AzuriteContainer _container = new AzuriteBuilder()
        .WithCommand("--skipApiVersionCheck")
        .WithPortBinding(10000, 10000) // map blob
        .WithPortBinding(10001, 10001) // map queue
        .WithPortBinding(10002, 10002) // map table
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(10000))
        .Build();

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await _container.StartAsync();

        var connectionString = _container.GetConnectionString();
        TestConfiguration.AzuriteConnectionString = connectionString;

        var azureService = new AzureService(connectionString);
        var storage = new Storage(azureService, [new AzureStorageInitializer()], new Logger<Storage>(new NullLoggerFactory()));
        await storage.InitializeAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}