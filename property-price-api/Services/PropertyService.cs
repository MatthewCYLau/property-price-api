﻿using System.Linq.Expressions;
using AutoMapper;
using CsvHelper;
using System.Globalization;
using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Helpers;
using property_price_api.Models;

namespace property_price_api.Services
{

    public interface IPropertyService
    {
        Task<List<PropertyDto>> GetProperties(DateTime? startDate, DateTime? endDate);
        Task<PropertyDto?> GetPropertyById(string? id);
        Task<Result<CreatePropertyResponse>> CreateProperty(CreatePropertyRequest createPropertyDto);
        Task<bool> UpdatePropertyById(string? id, UpdatePropertyRequest updatePropertyRequest);
        Task DeletePropertyById(string? id);
        Task<PriceAnalysisResponse> GeneratePriceAnalysisByPropertyId(string? id);
        Task CreateSeedProperties();
        public IEnumerable<T> ReadCSV<T>(Stream file);
    }

    public class PropertyService : IPropertyService
    {
        private const int PRICE_LIMIT = 1_000_000;
        private static readonly int CREATE_PROPERTY_DAILY_LIMIT = 10;
        private readonly ILogger _logger;
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PropertyService(
            ILogger<PropertyService> logger,
            MongoDbContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<PropertyDto>> GetProperties(DateTime? startDate, DateTime? endDate)
        {
            Expression<Func<Property, bool>> expression;
            if (startDate != null && endDate != null)
            {
                expression = x => x.Created >= startDate && x.Created <= endDate;
            }
            else
            {
                expression = _ => true;
            }

            var properties = await _context.Properties.Aggregate()
                .Match(expression)
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

        public async Task<Result<CreatePropertyResponse>> CreateProperty(CreatePropertyRequest createPropertyRequest)
        {
            var count = await GetPropertiesCreatedTodayCount();

            if (count > CREATE_PROPERTY_DAILY_LIMIT)
            {
                return Result<CreatePropertyResponse>.Failure(CreatePropertyErrors.DailyLimitReached(CREATE_PROPERTY_DAILY_LIMIT));
            }
            var property = _mapper.Map<Property>(createPropertyRequest);
            property.Created = DateTime.Now;
            property.AvatarUrl = new Random().Next(1, 4).ToString();
            var httpContext = _httpContextAccessor.HttpContext;
            var userDto = (Task<UserDto>)httpContext.Items["User"];
            property.UserId = userDto.Result.Id;

            if (createPropertyRequest.AskingPrice > PRICE_LIMIT)
            {
                throw new CustomException("Property is too expensive!");
            }

            await _context.Properties.InsertOneAsync(property);
            var createPropertyResponse = _mapper.Map<CreatePropertyResponse>(property);

            return Result<CreatePropertyResponse>.Success(createPropertyResponse);
        }


        public async Task<bool> UpdatePropertyById(string? id, UpdatePropertyRequest updatePropertyRequest)
        {

            var filter = Builders<Property>.Filter.Where(x => x.Id == id);
            var update = Builders<Property>.Update
                .Set(x => x.ListingUrl, updatePropertyRequest.ListingUrl)
                .Set(x => x.AskingPrice, updatePropertyRequest.AskingPrice)
                .Set(x => x.Address, updatePropertyRequest.Address)
                .Set(x => x.AvatarUrl, updatePropertyRequest.AvatarUrl);

            var options = new FindOneAndUpdateOptions<Property>();
            await _context.Properties.FindOneAndUpdateAsync(filter, update, options);

            return true;
        }

        public async Task DeletePropertyById(string? id)
        {
            await _context.Properties.DeleteOneAsync(x => x.Id == id);
            await _context.PriceSuggestions.DeleteManyAsync(x => x.PropertyId == id);
        }

        public async Task<PriceAnalysisResponse> GeneratePriceAnalysisByPropertyId(string? propertyId)
        {
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

        public async Task CreateSeedProperties()
        {
            _logger.LogInformation("Creating seed properties...");
            var count = await _context.Properties.EstimatedDocumentCountAsync();
            if (count != 0)
            {
                _logger.LogInformation("Collection has {0} documents. Skip creation of seed properties.", count);
                return;
            }

            var placeholderEmail = UserConstants.PlaceholderUserEmail;
            var plaerholderUser = await _context.Users.Find(x => x.Email == placeholderEmail).FirstOrDefaultAsync();
            if (plaerholderUser is null)
            {
                _logger.LogInformation("Creating placeholder user {0}", placeholderEmail);
                var newUser = new User
                {
                    Email = placeholderEmail,
                    Password = UserConstants.PlaceholderUserPassword,
                    UserType = UserTypes.Renter,
                    Created = DateTime.Now
                };
                await _context.Users.InsertOneAsync(newUser);
                _logger.LogInformation("Created placeholder user: {0}", newUser.Id);

                List<CreatePropertyRequest> requests =
                [
                   new()
                   {
                       ListingUrl = "https://www.rightmove.co.uk/properties/141178922#/?channel=RES_BUY",
                       Address = "Cardinal Close, Worcester Park",
                       AskingPrice = 555_000 },
                    new()
                    {
                       ListingUrl = "https://www.rightmove.co.uk/properties/137372225#/?channel=RES_BUY",
                       Address = "Inveresk Gardens, Worcester Park, KT4",
                       AskingPrice = 600_000 },
                     new()
                     {
                       ListingUrl = "https://www.rightmove.co.uk/properties/86360589#/?channel=RES_BUY",
                       Address = "Edenfield Gardens, Worcester Park, Surrey, KT4",
                       AskingPrice = 855_000 }
                ];

                foreach (CreatePropertyRequest request in requests)
                {
                    var newProperty = new Property
                    {
                        Created = DateTime.Now,
                        AskingPrice = request.AskingPrice,
                        Address = request.Address,
                        ListingUrl = request.ListingUrl,
                        AvatarUrl = new Random().Next(1, 4).ToString(),
                        UserId = newUser.Id
                    };

                    await _context.Properties.InsertOneAsync(newProperty);
                    _logger.LogInformation("Created placeholder property: {0}", newProperty.Id);
                }

            }
            else
            {
                _logger.LogInformation("Placeholder data already exist.");
            }

        }

        public IEnumerable<T> ReadCSV<T>(Stream file)
        {
            var reader = new StreamReader(file);
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<T>();
            return records;
        }

        private async Task<long> GetPropertiesCreatedTodayCount()
        {
            Expression<Func<Property, bool>> expression = x => x.Created > DateTime.Today;
            var count = await _context.Properties.CountDocumentsAsync(Builders<Property>.Filter.Where(expression));
            _logger.LogInformation("Properties created today count: {0}", count);
            return count;
        }
    }
}
