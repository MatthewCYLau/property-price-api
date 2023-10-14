using System.Linq.Expressions;
using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
    public interface IPriceSuggestionService
    {
        Task<PriceSuggestionResponse> GetPriceSuggestions(string? propertyId, int? page, int? pageSize);
        Task<PriceSuggestion?> GetPriceSuggestionById(string id);
        Task CreatePriceSuggestion(PriceSuggestion priceSuggestion);
        Task DeletePriceSuggestionById(string id);

    }

    public class PriceSuggestionService: IPriceSuggestionService
    {
        private readonly MongoDbContext _context;
        private readonly IPropertyService _propertyService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PriceSuggestionService(
           MongoDbContext context,
           IPropertyService propertyService,
           IHttpContextAccessor httpContextAccessor
           )

        {
            _context = context;
            _propertyService = propertyService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task CreatePriceSuggestion(PriceSuggestion priceSuggestion)
        {
            
            var httpContext = _httpContextAccessor.HttpContext;
            var userDto = (Task<UserDto>)httpContext!.Items["User"];
            var userId = userDto.Result.Id;
            
            if (_propertyService.GetPropertyById(priceSuggestion.PropertyId).Result == null)
            {
                throw new CustomException("Invalid property ID");
            }
            
            if (GetPriceSuggestionByPropertyIdByUserId(priceSuggestion.PropertyId, userId).Result != null)
            {
                throw new CustomException("User has already created price suggestion for property");
            }
            
            priceSuggestion.Created = DateTime.Now;
            priceSuggestion.UserId = userId;
            
            await _context.PriceSuggestions.InsertOneAsync(priceSuggestion);
        }

        public async Task<PriceSuggestion> GetPriceSuggestionById(string id)
        {
            var suggestion = await _context.PriceSuggestions.Find(x => x.Id == id).FirstOrDefaultAsync();
            return suggestion;
        }
        
        private async Task<PriceSuggestion> GetPriceSuggestionByPropertyIdByUserId(string propertyId, string userId)
        {
            Expression<Func<PriceSuggestion, bool>> filter = x => x.PropertyId == propertyId && x.UserId == userId;
            var suggestion = await _context.PriceSuggestions.Find(filter).FirstOrDefaultAsync();
            return suggestion;
        }

        public async Task DeletePriceSuggestionById(string id) =>
          await _context.PriceSuggestions.DeleteOneAsync(x => x.Id == id);

        public async Task<PriceSuggestionResponse> GetPriceSuggestions(string? propertyId, int? page, int? pageSize = 5)
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

            var totalRecordsCount = await _context.PriceSuggestions.Find(expression).CountDocumentsAsync();

            var priceSuggestions = await _context.PriceSuggestions.Aggregate()
                .Match(expression)
                .Lookup(CollectionNames.PropertiesCollection, "PropertyId", "_id", @as: "Property")
                .Skip((long)((page - 1) * pageSize))
                .Limit((long)pageSize)
                .Unwind("Property")
                .As<PriceSuggestion>()
                .ToListAsync();

            return new PriceSuggestionResponse( new PaginationMetadata((int)totalRecordsCount, currentPage: (int)page, (int)(totalRecordsCount / pageSize)), priceSuggestions);
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

