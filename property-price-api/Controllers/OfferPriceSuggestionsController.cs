﻿using property_price_api.Helpers;
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
        public async Task<List<OfferPriceSuggestion>> GetOfferPriceSuggestions([FromQuery] OfferPriceSuggestionQueryParameters parameters) =>
            await _offerPriceSuggestionService.GetOfferPriceSuggestions(parameters.PropertyId);

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Response>> CreateOfferPriceSuggestion(OfferPriceSuggestion offerPriceSuggestion)
        {

            await _offerPriceSuggestionService.CreateOfferPriceSuggestion(offerPriceSuggestion);

            return CreatedAtAction(nameof(GetOfferPriceSuggestions), new { id = offerPriceSuggestion.Id }, offerPriceSuggestion);
        }

        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeletePropertyPriceSuggestionById(string id)
        {
            var suggestion = await _offerPriceSuggestionService.GetOfferPriceSuggestionById(id);

            if (suggestion is null)
            {
                return NotFound();
            }

            await _offerPriceSuggestionService.DeleteOfferPriceSuggestionById(id);

            return NoContent();

        }
    }
}

