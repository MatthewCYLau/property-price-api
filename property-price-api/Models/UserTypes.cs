using System.Collections.ObjectModel;

namespace property_price_api.Models
{
	public static class UserTypes
	{
        private const string Renter = "Renter";
        private const string FirstTimeBuyer = "FirstTimeBuyer";
        private const string HomeOwner = "HomeOwner";
        private const string Landlord = "Landlord";

        public static ReadOnlyCollection<string> UserTypesList { get; } = new(new[]
        {
            Renter,
            FirstTimeBuyer,
            HomeOwner,
            Landlord
        });
    }

}

