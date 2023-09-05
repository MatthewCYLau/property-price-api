using System.Collections.ObjectModel;

namespace property_price_api.Models
{
	public static class UserTypes
	{
        public const string Renter = "Renter";
        public const string FirstTimeBuyer = "FirstTimeBuyer";
        public const string HomeOwner = "HomeOwner";
        public const string Landlord = "Landlord";

        private static readonly ReadOnlyCollection<string> _userTypes =
        new ReadOnlyCollection<string>(new[]
        {
            Renter,
            FirstTimeBuyer,
            HomeOwner,
            Landlord
        });

        public static ReadOnlyCollection<string> UserTypesList
        {
            get { return _userTypes; }
        }
    }

}

