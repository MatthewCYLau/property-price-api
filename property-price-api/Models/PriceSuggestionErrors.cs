namespace property_price_api;

public class PriceSuggestionErrors
{
    public static Error InvalidPropertyId(string id) => new Error(
    "PriceSuggestion.InvalidPropertyId", $"Invalid property ID {id}");

    public static readonly Error UserAlreadyCreatedPriceSuggestion = new(
        "PriceSuggestion.UserAlreadyCreatedPriceSuggestion", "User has already created price suggestion for property");
}
