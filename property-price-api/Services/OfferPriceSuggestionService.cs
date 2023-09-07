using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
    public interface IOfferPriceSuggestionService
    {
        Task<List<OfferPriceSuggestion>> GetOfferPriceSuggestions();
        Task CreateOfferPriceSuggestion(OfferPriceSuggestion offerPriceSuggestion);
    }

    public class OfferPriceSuggestionService: IOfferPriceSuggestionService
    {
        private readonly MongoDbContext _context;

        public OfferPriceSuggestionService(
           MongoDbContext context
)
        {
            _context = context;

        }

        public async Task CreateOfferPriceSuggestion(OfferPriceSuggestion offerPriceSuggestion)
        {
            await _context.OfferPriceSuggestions.InsertOneAsync(offerPriceSuggestion);
        }

        public async Task<List<OfferPriceSuggestion>> GetOfferPriceSuggestions()
        {
            var _offerPriceSuggestions = await _context.OfferPriceSuggestions.Find(_ => true).ToListAsync();
            return _offerPriceSuggestions;
        }
    }
}

