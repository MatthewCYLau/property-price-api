namespace property_price_api.Models
{
	public class UpdateUserRequest: AuthenticateRequest
	{

        public string? Password { get; set; }

        public string UserType { get; set; } = null!;

    }
}

