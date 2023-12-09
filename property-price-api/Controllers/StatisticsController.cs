using Microsoft.AspNetCore.Mvc;
using property_price_api.Helpers;
using property_price_api.Models;
using property_price_api.Services;

namespace property_price_api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {

        private readonly IPriceSuggestionService _priceSuggestionService;

        public StatisticsController(IPriceSuggestionService priceSuggestionService)
        {
            _priceSuggestionService = priceSuggestionService;
        }

        [Authorize]
        [HttpGet("price-suggestions")]
        public async Task<ActionResult<PriceSuggestionsStatisticsResponse>> GetPriceSuggestionsStatistics()
        {
            
            return await _priceSuggestionService.GetPriceSuggestionsStatistics();
        }
    }
}

