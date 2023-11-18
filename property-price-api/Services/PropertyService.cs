using System.Text.Json;
using AutoMapper;
using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Helpers;
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
        void CreateSeedProperties();
    }

    public class Post
    {
        public int userId { get; set; }
        public int id { get; set; }
        public required string title { get; set; }
        public required string body { get; set; }
    }

    public class PropertyService: IPropertyService
	{

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PropertyService(
            IHttpClientFactory httpClientFactory,
            MongoDbContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<PropertyDto>> GetProperties()
        {
            var properties = await _context.Properties.Aggregate()
            .Lookup(CollectionNames.UsersCollection, "UserId", "_id", @as: "User")
            .Unwind("User")
            .As<Property>()
            .SortByDescending(i => i.Created)
            .ToListAsync();

            var propertiesDto = _mapper.Map<List<PropertyDto>>(properties);
            return propertiesDto;
        }
       
        public async Task<PropertyDto?> GetPropertyById(string? id)
        {
            var property = await _context.Properties.Aggregate()
            .Match(x => x.Id == id)
            .Lookup(CollectionNames.UsersCollection, "UserId", "_id", @as: "User")
            .Unwind("User")
            .As<Property>()
            .SingleOrDefaultAsync();

            var propertyDto = _mapper.Map<PropertyDto>(property);
            return propertyDto;
        }    

        public async Task<CreatePropertyResponse> CreateProperty(CreatePropertyRequest createPropertyRequest)
        {
            var property = _mapper.Map<Property>(createPropertyRequest);
            property.Created = DateTime.Now;
            property.AvatarId = new Random().Next(1, 4);
            var httpContext = _httpContextAccessor.HttpContext;
            var userDto = (Task<UserDto>)httpContext.Items["User"];
            property.UserId = userDto.Result.Id;

            if (createPropertyRequest.AskingPrice > 1000000)
            {
                throw new CustomException("Property is too expensive!");
            }

            await _context.Properties.InsertOneAsync(property);
            var createPropertyResponse = _mapper.Map<CreatePropertyResponse>(property);

            return createPropertyResponse;
        }
            

        public async Task UpdatePropertyById(string? id, Property property) =>
            await _context.Properties.ReplaceOneAsync(x => x.Id == id, property);

        public async Task DeletePropertyById(string? id)
        {
            await _context.Properties.DeleteOneAsync(x => x.Id == id);
            await _context.PriceSuggestions.DeleteManyAsync(x => x.PropertyId == id);
        }

        private async Task<List<Post>> GetJsonPlaceholderPosts()
        {
            var httpClient = _httpClientFactory.CreateClient("JsonPlaceholder");
            var httpResponseMessage = await httpClient.GetAsync(
                "posts?_start=0&_limit=5");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();

                 return (List<Post>)await JsonSerializer.DeserializeAsync
                    <IEnumerable<Post>>(contentStream);
            }

            return new List<Post>();
        }
           

        public async Task<PriceAnalysisResponse> GeneratePriceAnalysisByPropertyId(string? propertyId)
        {
            var posts = await GetJsonPlaceholderPosts();
            posts.ForEach(n => Console.WriteLine(n.body));
            var priceSuggestions = await _context.PriceSuggestions.Find(x => x.PropertyId == propertyId).ToListAsync();

            if (!priceSuggestions.Any())
            {
                return new PriceAnalysisResponse(-1, 0);
            }
            var percentages = priceSuggestions.Select(c => c.DifferenceInPercentage).ToList();
            var askingPrice = GetPropertyById(propertyId).Result!.AskingPrice;           
            var meanSuggestedPrice = Calculations.MeanSuggestedPrice(percentages, askingPrice);
            var percentageDifferenceFromAskingPrice = meanSuggestedPrice * 100 / askingPrice - 100;
            return new PriceAnalysisResponse(meanSuggestedPrice, percentageDifferenceFromAskingPrice);
        }

        public void CreateSeedProperties()
        {
            Console.WriteLine("Creating seed properties...");
        }
    }
}
