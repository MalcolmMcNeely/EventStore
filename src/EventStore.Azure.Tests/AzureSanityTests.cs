using Azure.Storage.Blobs;
using EventStore.Azure.Azure;
using EventStore.Azure.Extensions;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.Utility;

namespace EventStore.Azure.Tests;

public class AzureSanityTests : IntegrationTest
{
    BlobClient _blobClient;
    BinaryData _binaryData;

    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithAzureServices()
            .Build();
    }

    [SetUp]
    public void Setup()
    {
        var azureService = GetService<AzureService>();
        var blobContainerClient = azureService.BlobServiceClient.GetBlobContainerClient(Defaults.AggregateRoot.ContainerName);
        _blobClient = blobContainerClient.GetBlobClient("test-blob");
        _binaryData = BinaryData.FromString("test content");
    }

    [Test]
    public async Task UploadOnlyIfNotCreated_should_only_upload_once_when_many_calls_are_made()
    {
        var successfulUploads = 0;
        var timesToInvoke = 2000;

        await TestUtility.InvokeManyAsync(async () =>
        {
            if (await _blobClient.UploadOnlyIfNotCreated(_binaryData))
            {
                successfulUploads++;
            }
        }, timesToInvoke);

        Assert.That(successfulUploads, Is.EqualTo(1));
    }
}