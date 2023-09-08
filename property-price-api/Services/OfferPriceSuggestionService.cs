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
        private readonly IPropertyService _propertyService;

        public OfferPriceSuggestionService(
           MongoDbContext context,
           IPropertyService propertyService
)
        {
            _context = context;
            _propertyService = propertyService;

        }

        public async Task CreateOfferPriceSuggestion(OfferPriceSuggestion offerPriceSuggestion)
        {
            if (_propertyService.GetPropertyById(offerPriceSuggestion.PropertyId).Result == null)
            {
                throw new CustomException("Invalid property ID");
            }
            await _context.OfferPriceSuggestions.InsertOneAsync(offerPriceSuggestion);
        }

        public async Task<List<OfferPriceSuggestion>> GetOfferPriceSuggestions()
        {
            var _offerPriceSuggestions = await _context.OfferPriceSuggestions.Find(_ => true).ToListAsync();
            return _offerPriceSuggestions;
        }
    }
}

