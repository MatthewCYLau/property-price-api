namespace property_price_api.Models
{
	public class CustomException : Exception
    {
        public CustomException()
        {
        }

        public CustomException(string message)
            : base(message)
        {
        }
    }
}

