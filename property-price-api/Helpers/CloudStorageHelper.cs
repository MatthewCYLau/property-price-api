using Google.Cloud.Storage.V1;

namespace property_price_api.Helpers;

public static class CloudStorageHelper
{
    public static Stream DownloadFile(
        string bucketName,
        string objectName)
    {
        var storage = StorageClient.Create();
        Stream stream = new MemoryStream();
        storage.DownloadObject(bucketName, objectName, stream);
        return stream;
    }

    public static (string, string) GetBucketAndObjectNamesFromObjectUrl(string objectUrl)
    {
        var objectUrlParts = objectUrl.Replace("https://storage.googleapis.com/", "").Split("/");
        return (objectUrlParts[0], objectUrlParts[1]);
    }
}
