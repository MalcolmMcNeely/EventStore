using EventStore.Azure.Initialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EventStore.Azure.Tests;

[SetUpFixture]
public class TestFixture
{
    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        var azureService = new AzureService();
        var storage = new Storage(azureService,
            [new AzureStorageInitializer()],
            new Logger<Storage>(new NullLoggerFactory()));

        await storage.InitializeAsync();
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        var azureService = new AzureService();
        var storage = new Storage(azureService,
            [new AzureStorageInitializer()],
            new Logger<Storage>(new NullLoggerFactory()));

        await storage.DestroyAsync();
    }
}