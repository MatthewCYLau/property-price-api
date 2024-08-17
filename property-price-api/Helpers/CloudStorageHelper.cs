using Google.Cloud.Storage.V1;

namespace property_price_api.Helpers;

public static class CloudStorageHelper
{
    public static void DownloadFile(
        string bucketName,
        string objectName,
        string outputPath)
    {
        var storage = StorageClient.Create();
        using var outputFile = File.OpenWrite(outputPath);
        storage.DownloadObject(bucketName, objectName, outputFile);
    }

    public static (string, string) GetBucketAndObjectNamesFromObjectUrl(string objectUrl)
    {
        var objectUrlParts = objectUrl.Replace("https://storage.googleapis.com/", "").Split("/");
        return (objectUrlParts[0], objectUrlParts[1]);
    }
}
