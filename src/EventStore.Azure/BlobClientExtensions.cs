using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace EventStore.Azure;

public static class BlobClientExtensions
{
    public static async Task<bool> UploadOnlyIfNotCreated(this BlobClient blobClient, BinaryData binaryData)
    {
        try
        {
            await blobClient.UploadAsync(binaryData, new BlobUploadOptions
            {
                Conditions = new BlobRequestConditions { IfNoneMatch = new ETag("*") }
            });
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
        {
            return false;
        }

        return true;
    }

    public static async Task<bool> UploadWithLeaseAsync(this BlobClient blobClient, BinaryData binaryData)
    {
        try
        {
            var leaseClient = blobClient.GetBlobLeaseClient();
            var lease = await leaseClient.AcquireAsync(TimeSpan.FromSeconds(15));
            var uploadOptions = new BlobUploadOptions
            {
                Conditions = new BlobRequestConditions { LeaseId = lease.Value.LeaseId },
            };

            await blobClient.UploadAsync(binaryData, uploadOptions);
            await leaseClient.ReleaseAsync();
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.LeaseIdMismatchWithLeaseOperation || ex.ErrorCode == BlobErrorCode.LeaseAlreadyPresent)
        {
            return false;
        }

        return true;
    }
}