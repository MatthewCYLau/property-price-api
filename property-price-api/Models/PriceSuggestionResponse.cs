namespace property_price_api.Models
{
	public class PriceSuggestionResponse
	{
        public PriceSuggestionResponse(PaginationMetadata paginationMetadata, List<PriceSuggestion> priceSuggestions)
        {
            PaginationMetadata = paginationMetadata;
            PriceSuggestions = priceSuggestions;
        }

        public PaginationMetadata PaginationMetadata { get; set; }
        public List<PriceSuggestion> PriceSuggestions { get; set; }
    }
}

