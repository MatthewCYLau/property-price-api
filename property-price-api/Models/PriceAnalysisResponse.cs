namespace property_price_api.Models
{
	public class PriceAnalysisResponse
	{
        public PriceAnalysisResponse(int suggestedPrice)
        {
            SuggestedPrice = suggestedPrice;
        }

        public int SuggestedPrice { get; set; }

    }
}

