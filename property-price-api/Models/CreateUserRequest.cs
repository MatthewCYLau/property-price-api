using System.ComponentModel.DataAnnotations;

namespace property_price_api.Models
{
	public class CreateUserRequest
	{
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "User type is required")]
        public string UserType { get; set; } = null!;
    }
}

