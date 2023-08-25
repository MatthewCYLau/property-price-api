using System.ComponentModel.DataAnnotations;

namespace property_price_api.Models
{

    public class AuthenticateRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

