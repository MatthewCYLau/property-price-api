using System.Linq.Expressions;
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
            await _context.PriceSuggestions.InsertOneAsync(priceSuggestion);
        }

        public async Task<PriceSuggestion> GetPriceSuggestionById(string id)
        {
            var suggestion = await _context.PriceSuggestions.Find(x => x.Id == id).FirstOrDefaultAsync();
            return suggestion;
        }

        public async Task DeletePriceSuggestionById(string id) =>
          await _context.PriceSuggestions.DeleteOneAsync(x => x.Id == id);

        public async Task<List<PriceSuggestion>> GetPriceSuggestions(string? propertyId)
        {
            Expression<Func<PriceSuggestion,bool>> expression;
            if (propertyId != null)
            {
                ValidatePropertyId(propertyId);
                 expression = x => x.PropertyId == propertyId;
            }
            else
            {
                expression = _ => true;
            }
            var priceSuggestions = await _context.PriceSuggestions.Aggregate()
                .Match(expression)
                .Lookup("properties", "PropertyId", "_id", @as: "Property")
                .Unwind("Property")
                .As<PriceSuggestion>()
                .ToListAsync();
            return priceSuggestions;
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

