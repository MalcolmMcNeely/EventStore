using Azure.Storage.Blobs;
using EventStore.Azure.Azure;
using EventStore.Projections;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.SimpleTestDomain;
using EventStore.Utility;

namespace EventStore.Azure.Tests.Projections;

public class ProjectionRebuilderTests : IntegrationTest
{
    IProjectionRepository<TestProjection> _projectionRepository;
    TestProjection _initialProjection;
    BlobClient _blobClient;

    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithAzureServices()
            .WithTestDomain()
            .Build();
    }

    [SetUp]
    public async Task Setup()
    {
        _projectionRepository = GetService<IProjectionRepository<TestProjection>>();
        var blobContainerClient = GetService<AzureService>().BlobServiceClient.GetBlobContainerClient(Defaults.Projections.ContainerName);
        _blobClient = blobContainerClient.GetBlobClient($"{nameof(TestProjection)}/{nameof(TestProjection)}");

        await SendEventAsync(new TestEvent { Data = "test" });

        _initialProjection = (await _projectionRepository.LoadAsync(nameof(TestProjection)))!;
    }

    [Test]
    public async Task when_projection_is_deleted_it_rebuilds_from_the_event_stream()
    {
        await _blobClient.DeleteAsync();
        await Wait.UntilAsync(async () => !await _blobClient.ExistsAsync());

        var newProjection = (await _projectionRepository.LoadAsync(nameof(TestProjection)))!;

        Assert.That(newProjection.Data, Is.EqualTo(_initialProjection.Data));
    }
}