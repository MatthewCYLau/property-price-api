namespace property_price_api.Models
{
	public class PriceAnalysisResponse
	{
		public PriceAnalysisResponse(int suggestedPrice, int percentageDifferenceFromAskingPrice)
		{
			SuggestedPrice = suggestedPrice;
			PercentageDifferenceFromAskingPrice = percentageDifferenceFromAskingPrice;
		}

		public int SuggestedPrice { get; set; }
        public int PercentageDifferenceFromAskingPrice { get; set; }

    }
}

