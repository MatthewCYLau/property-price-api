namespace property_price_api.Models
{
	public class UpdateUserRequest: AuthenticateRequest
	{
		public User ToUser(string? id, string email, string password)
		{
			return new User(id, email, password);
		}
	}
}

