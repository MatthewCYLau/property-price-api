namespace property_price_api.Models
{
	public class CreateJobRequest
	{
        public string Postcode { get; set; } = null!;

        public CreateJobRequest(string postcode)
        {
            Postcode = postcode;
        }
    }
}

