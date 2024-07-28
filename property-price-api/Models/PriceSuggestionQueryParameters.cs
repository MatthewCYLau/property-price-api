namespace property_price_api.Models
{
    public class PriceSuggestionQueryParameters
    {

        public string? PropertyId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;

    }
}

