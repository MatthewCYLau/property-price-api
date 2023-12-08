namespace property_price_api.Models
{
	public class PriceSuggestionsStatisticsResponse
	{
        public int AboveAsking { get; set; }
        public int Asking { get; set; }
        public int BelowAsking { get; set; }

        public PriceSuggestionsStatisticsResponse(int aboveAsking, int asking, int belowAsking)
        {
            AboveAsking = aboveAsking;
            Asking = asking;
            BelowAsking = belowAsking;
        }
    }
}

