using System.Text.RegularExpressions;

namespace property_price_api.Helpers
{
	public class StringHelpers
	{
		public static string UpperCaseWord(string input)
		{
			return input[0].ToString().ToUpper() + input.Substring(1);
        }

		public static bool IsValidateUkPostcode(string postcode)
		{
            return Regex.IsMatch(postcode, @"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})");
        }
	}
}

