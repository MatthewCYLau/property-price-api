using System.ComponentModel.DataAnnotations;

namespace property_price_api.Models
{
	public class CreateUserDto
	{
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}

