using Azure.Storage.Blobs;
using EventStore.Azure.Azure;
using EventStore.Projections;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains;
using EventStore.Testing.TestDomains.Simple;
using EventStore.Utility;

namespace EventStore.Azure.Tests.Projections;

public class ProjectionRebuilderTests : IntegrationTest
{
    IProjectionRepository<SimpleProjection> _projectionRepository;
    BlobClient _blobClient;

    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithAzureServices()
            .WithSimpleDomain()
            .Build();
    }

    [SetUp]
    public async Task Setup()
    {
        await SendEventAsync(new SimpleEvent { Data = "test" });

        var blobContainerClient = GetService<AzureService>().BlobServiceClient.GetBlobContainerClient(Defaults.Projections.ContainerName);
        _blobClient = blobContainerClient.GetBlobClient($"{nameof(SimpleProjection)}/{nameof(SimpleProjection)}");
        await Wait.UntilAsync(async () => await _blobClient.ExistsAsync());
    }

    [Test]
    public async Task when_projection_is_deleted_it_rebuilds_from_the_event_stream()
    {
        await _blobClient.DeleteAsync();
        await Wait.UntilAsync(async () => !await _blobClient.ExistsAsync());

        _projectionRepository = GetService<IProjectionRepository<SimpleProjection>>();
        var rebuiltProjection = (await _projectionRepository.LoadAsync(nameof(SimpleProjection)))!;

        await Verify(rebuiltProjection);
    }
}