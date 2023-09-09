using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
    public interface IOfferPriceSuggestionService
    {
        Task<List<OfferPriceSuggestion>> GetOfferPriceSuggestions(string propertyId);
        Task<OfferPriceSuggestion?> GetOfferPriceSuggestionById(string id);
        Task CreateOfferPriceSuggestion(OfferPriceSuggestion offerPriceSuggestion);
        Task DeleteOfferPriceSuggestionById(string id);

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

        public async Task<OfferPriceSuggestion> GetOfferPriceSuggestionById(string id)
        {
            var suggestion = await _context.OfferPriceSuggestions.Find(x => x.Id == id).FirstOrDefaultAsync();
            return suggestion;
        }

        public async Task DeleteOfferPriceSuggestionById(string id) =>
          await _context.OfferPriceSuggestions.DeleteOneAsync(x => x.Id == id);

        public async Task<List<OfferPriceSuggestion>> GetOfferPriceSuggestions(string propertyId)
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


        private void ValidatePropertyId(string propertyId)
        {
            if (_propertyService.GetPropertyById(propertyId).Result == null)
            {
                throw new CustomException("Invalid property ID");
            }
        }
    }
}

