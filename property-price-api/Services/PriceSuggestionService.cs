using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
    public interface IPriceSuggestionService
    {
        Task<List<PriceSuggestion>> GetPriceSuggestions(string? propertyId);
        Task<PriceSuggestion?> GetPriceSuggestionById(string id);
        Task CreatePriceSuggestion(PriceSuggestion priceSuggestion);
        Task DeletePriceSuggestionById(string id);

    }

    public class PriceSuggestionService: IPriceSuggestionService
    {
        private readonly MongoDbContext _context;
        private readonly IPropertyService _propertyService;

        public PriceSuggestionService(
           MongoDbContext context,
           IPropertyService propertyService
)
        {
            _context = context;
            _propertyService = propertyService;

        }

        public async Task CreatePriceSuggestion(PriceSuggestion priceSuggestion)
        {
            if (_propertyService.GetPropertyById(priceSuggestion.PropertyId).Result == null)
            {
                throw new CustomException("Invalid property ID");
            }
            await _context.OfferPriceSuggestions.InsertOneAsync(priceSuggestion);
        }

        public async Task<PriceSuggestion> GetPriceSuggestionById(string id)
        {
            var suggestion = await _context.OfferPriceSuggestions.Find(x => x.Id == id).FirstOrDefaultAsync();
            return suggestion;
        }

        public async Task DeletePriceSuggestionById(string id) =>
          await _context.OfferPriceSuggestions.DeleteOneAsync(x => x.Id == id);

        public async Task<List<PriceSuggestion>> GetPriceSuggestions(string? propertyId)
        {
            
            if (propertyId != null)
            {
                ValidatePropertyId(propertyId);
                return await _context.OfferPriceSuggestions.Find(x => x.PropertyId == propertyId).ToListAsync();

            } else
            {
                return await _context.OfferPriceSuggestions.Find(_ => true).ToListAsync();
            }
            
        }


        private void ValidatePropertyId(string? propertyId)
        {
            if (_propertyService.GetPropertyById(propertyId).Result == null)
            {
                throw new CustomException("Invalid property ID");
            }
        }
    }
}

