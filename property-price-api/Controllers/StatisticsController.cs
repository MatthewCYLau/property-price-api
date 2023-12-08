using Microsoft.AspNetCore.Mvc;
using property_price_api.Helpers;
using property_price_api.Models;

namespace property_price_api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {

        [Authorize]
        [HttpGet("price-suggestions")]
        public ActionResult<PriceSuggestionsStatisticsResponse> GetPriceSuggestionsStatistics()
        {
            
            return Ok(new PriceSuggestionsStatisticsResponse(0, 0, 0));
        }
    }
}

