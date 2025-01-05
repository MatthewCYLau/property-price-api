namespace property_price_cosmos_db.Models;

public class BlobClientErrors
{
    public static Error InvalidBlobId(string blobId) => new(
    "BlobClientErrors.InvalidBlobId", $"Invalid blob ID {blobId}");
}
