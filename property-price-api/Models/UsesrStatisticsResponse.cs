namespace property_price_api.Models
{
	public class UsersStatisticsResponse
	{
        public int RenterCount { get; set; }

        public UsersStatisticsResponse(int renterCount)
        {
            RenterCount = renterCount;
        }
    }
}

