namespace EventStore.Azure.Tests;

public static class AzureServiceExtensions
{
    public static void DeleteExistingData(this AzureService azureService)
    {
        azureService.BlobServiceClient.DeleteBlobContainer(BlobContainerConstants.AggregateRootContainerName);
    }
}