using System.ComponentModel.DataAnnotations;

namespace property_price_api.Models
{
	public class UpdateNotificationRequest
	{
        [Required]
        public bool ReadStatus { get; set; }
    }
}

