namespace property_price_api.Models
{
	public class CreateJobResponse
	{
		public string JobId { get; set; }

        public CreateJobResponse(string jobId)
        {
            JobId = jobId;
        }
    }
}

