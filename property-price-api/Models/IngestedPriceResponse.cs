namespace property_price_api.Models
{
	public class IngestedPriceResponse
	{
        public string Postcode { get; set; }
        public int TransactionPrice { get; set; }

        public IngestedPriceResponse(string postcode, int transactionPrice)
        {
            Postcode = postcode;
            TransactionPrice = transactionPrice;
        }
    }
}

