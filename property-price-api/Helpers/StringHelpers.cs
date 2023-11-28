namespace property_price_api.Helpers
{
	public class StringHelpers
	{
		public static string UpperCaseWord(string input)
		{
			return input[0].ToString().ToUpper() + input.Substring(1);
        }
	}
}

