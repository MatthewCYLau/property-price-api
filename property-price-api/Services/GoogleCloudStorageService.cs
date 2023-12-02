using Google.Cloud.Storage.V1;

namespace property_price_api.Services
{

	public interface IGoogleCloudStorageService
    {
        Task<string> UploadFileAsync(IFormFile imageFile);
    }

    public class GoogleCloudStorageService: IGoogleCloudStorageService
	{
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly ILogger _logger;

        public GoogleCloudStorageService(IConfiguration configuration, ILogger<GoogleCloudStorageService> logger)
        {
            _storageClient = StorageClient.Create();
            _bucketName = configuration.GetValue<string>("GoogleCloudStorageBucketName");
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(IFormFile imageFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                _logger.LogInformation("Uploading image {0} to Google Cloud Storage bucket {1}", imageFile.FileName, _bucketName);
                var timestamp = DateTime.Now.ToFileTime();
                var dataObject = await _storageClient.UploadObjectAsync(_bucketName, string.Concat(timestamp, "-", imageFile.FileName), imageFile.ContentType, memoryStream);
                return dataObject.MediaLink;
            }
        }
    }
}

