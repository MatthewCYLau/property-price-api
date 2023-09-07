using property_price_api.Helpers;
using Microsoft.AspNetCore.Mvc;
using property_price_api.Models;
using property_price_api.Services;

namespace property_price_api.Controllers
{
    [ApiController]
    [Route("api/price-suggestions")]
    public class OfferPriceSuggestionsController : ControllerBase
	{
        private IOfferPriceSuggestionService _offerPriceSuggestionService;

        public OfferPriceSuggestionsController(IOfferPriceSuggestionService offerPriceSuggestionService)
        {
            _offerPriceSuggestionService = offerPriceSuggestionService;
        }


        [Authorize]
        [HttpGet]
        public async Task<List<OfferPriceSuggestion>> GetOfferPriceSuggestions() =>
            await _offerPriceSuggestionService.GetOfferPriceSuggestions();

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Response>> CreateOfferPriceSuggestion(OfferPriceSuggestion offerPriceSuggestion)
        {

            await _offerPriceSuggestionService.CreateOfferPriceSuggestion(offerPriceSuggestion);

            return CreatedAtAction(nameof(GetOfferPriceSuggestions), new { id = offerPriceSuggestion.Id }, offerPriceSuggestion);
        }
    }
}

