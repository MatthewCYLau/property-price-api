using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using property_price_api.Helpers;
using property_price_api.Models;
using property_price_api.Services;

namespace property_price_api.Controllers
{

    [EnableRateLimiting("fixed")]
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {

        private readonly IPriceSuggestionService _priceSuggestionService;
        private readonly IUserService _userService;


        public StatisticsController(IPriceSuggestionService priceSuggestionService, IUserService userService)
        {
            _priceSuggestionService = priceSuggestionService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet("price-suggestions")]
        public async Task<ActionResult<PriceSuggestionsStatisticsResponse>> GetPriceSuggestionsStatistics()
        {
            
            return await _priceSuggestionService.GetPriceSuggestionsStatistics();
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<ActionResult<UsersStatisticsResponse>> GetUsersStatistics()
        {

            return await _userService.GetUsersStatistics();
        }
    }
}

