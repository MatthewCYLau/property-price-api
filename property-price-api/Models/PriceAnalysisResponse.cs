namespace property_price_api.Models
{
	public class PriceAnalysisResponse
	{
        public PriceAnalysisResponse(decimal suggestedPrice)
        {
            SuggestedPrice = suggestedPrice;
        }

        public decimal SuggestedPrice { get; set; }

    }
}

