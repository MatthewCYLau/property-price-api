using AutoMapper;
using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{

    public interface IPropertyService
    {
        Task<List<PropertyDto>> GetProperties();
        Task<PropertyDto?> GetPropertyById(string? id);
        Task<CreatePropertyResponse> CreateProperty(CreatePropertyRequest createPropertyDto);
        Task UpdatePropertyById(string? id, Property property);
        Task DeletePropertyById(string? id);
        Task<PriceAnalysisResponse> GeneratePriceAnalysisByPropertyId(string? id);
    }

    public class PropertyService: IPropertyService
	{

        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PropertyService(
            MongoDbContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<PropertyDto>> GetProperties()
        {
            var properties = await _context.Properties.Aggregate()
            .Lookup("users", "UserId", "_id", @as: "User")
            .Unwind("User")
            .As<Property>()
            .ToListAsync();

            var propertiesDto = _mapper.Map<List<PropertyDto>>(properties);
            return propertiesDto;
        }
       
        public async Task<PropertyDto?> GetPropertyById(string? id)
        {
            var property = await _context.Properties.Aggregate()
            .Match(x => x.Id == id)
            .Lookup("users", "UserId", "_id", @as: "User")
            .Unwind("User")
            .As<Property>()
            .SingleOrDefaultAsync();

            var propertyDto = _mapper.Map<PropertyDto>(property);
            return propertyDto;
        }    

        public async Task<CreatePropertyResponse> CreateProperty(CreatePropertyRequest createPropertyRequest)
        {
            var _property = _mapper.Map<Property>(createPropertyRequest);
            var httpContext = _httpContextAccessor.HttpContext;
            var _userDto = (Task<UserDto>)httpContext.Items["User"];
            _property.UserId = _userDto.Result.Id;

            if (createPropertyRequest.AskingPrice > 100)
            {
                throw new CustomException("Property is too expensive!");
            }

            await _context.Properties.InsertOneAsync(_property);
            var createPropertyResponse = _mapper.Map<CreatePropertyResponse>(_property);

            return createPropertyResponse;
        }
            

        public async Task UpdatePropertyById(string? id, Property property) =>
            await _context.Properties.ReplaceOneAsync(x => x.Id == id, property);

        public async Task DeletePropertyById(string? id) =>
            await _context.Properties.DeleteOneAsync(x => x.Id == id);

        public async Task<PriceAnalysisResponse> GeneratePriceAnalysisByPropertyId(string? propertyId)
        {
            var priceSuggestions = await _context.OfferPriceSuggestions.Find(x => x.PropertyId == propertyId).ToListAsync();
            List<int> percentages = priceSuggestions.Select(c => c.DifferenceInPercentage).ToList();
            var askingPrice = GetPropertyById(propertyId).Result.AskingPrice;           
            var meanSuggestedPrice = percentages.Select(i => (decimal)i / 100 * askingPrice).Sum() / percentages.Count;
            return new PriceAnalysisResponse(meanSuggestedPrice);
        }
    }
}
