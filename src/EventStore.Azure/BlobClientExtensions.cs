using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace EventStore.Azure;

public static class BlobClientExtensions
{
    static readonly TimeSpan LeaseDuration = TimeSpan.FromSeconds(30);
    
    public static async Task<bool> UploadOnlyIfNotCreated(this BlobClient blobClient, BinaryData binaryData, CancellationToken cancellationToken = default)
    {
        try
        {
            await blobClient.UploadAsync(binaryData, new BlobUploadOptions
            {
                Conditions = new BlobRequestConditions { IfNoneMatch = new ETag("*") }
            }, cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
        {
            return false;
        }

        return true;
    }

    public static async Task<bool> UploadWithLeaseAsync(this BlobClient blobClient, BinaryData binaryData, CancellationToken token = default)
    {
        var leaseClient = blobClient.GetBlobLeaseClient();
        BlobLease? lease = null;

        try
        {
            lease = await leaseClient.AcquireAsync(LeaseDuration, cancellationToken: token);

            var uploadOptions = new BlobUploadOptions
            {
                Conditions = new BlobRequestConditions { LeaseId = lease.LeaseId },
            };

            await blobClient.UploadAsync(binaryData, uploadOptions, token);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.LeaseIdMismatchWithLeaseOperation ||
                                                ex.ErrorCode == BlobErrorCode.LeaseAlreadyPresent)
        {
            return false;
        }
        finally
        {
            if (lease?.LeaseId is not null)
            {
                await leaseClient.ReleaseAsync(new BlobRequestConditions { LeaseId = lease.LeaseId }, cancellationToken: token);
            }
        }

        return true;
    }
}