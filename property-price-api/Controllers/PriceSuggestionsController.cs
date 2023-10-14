using property_price_api.Helpers;
using Microsoft.AspNetCore.Mvc;
using property_price_api.Models;
using property_price_api.Services;

namespace property_price_api.Controllers
{
    [ApiController]
    [Route("api/price-suggestions")]
    public class PriceSuggestionsController : ControllerBase
	{
        private readonly IPriceSuggestionService _priceSuggestionService;

        public PriceSuggestionsController(IPriceSuggestionService priceSuggestionService)
        {
            _priceSuggestionService = priceSuggestionService;
        }


        [Authorize]
        [HttpGet]
        public async Task<PriceSuggestionResponse> GetPriceSuggestions([FromQuery] PriceSuggestionQueryParameters parameters) =>
            await _priceSuggestionService.GetPriceSuggestions(parameters.PropertyId, parameters.Page, parameters.PageSize);

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Response>> CreatePriceSuggestion(PriceSuggestion priceSuggestion)
        {
            await _priceSuggestionService.CreatePriceSuggestion(priceSuggestion);
            return CreatedAtAction(nameof(GetPriceSuggestions), new { id = priceSuggestion.Id }, priceSuggestion);
        }

        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeletePropertyPriceSuggestionById(string id)
        {
            var suggestion = await _priceSuggestionService.GetPriceSuggestionById(id);

            if (suggestion is null)
            {
                return NotFound();
            }

            await _priceSuggestionService.DeletePriceSuggestionById(id);

            return NoContent();

        }
    }
}

