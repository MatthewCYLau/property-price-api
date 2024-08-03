using System.Linq.Expressions;
using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
    public interface IPriceSuggestionService
    {
        Task<PriceSuggestionResponse> GetPriceSuggestions(string propertyId, int page, int pageSize);
        Task<PriceSuggestion?> GetPriceSuggestionById(string id);
        Task<Result> CreatePriceSuggestion(PriceSuggestion priceSuggestion);
        Task DeletePriceSuggestionById(string id);
        Task<PriceSuggestionsStatisticsResponse> GetPriceSuggestionsStatistics();
    }

    public class PriceSuggestionService : IPriceSuggestionService
    {
        private readonly MongoDbContext _context;
        private readonly IPropertyService _propertyService;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PriceSuggestionService(
           MongoDbContext context,
           IPropertyService propertyService,
           INotificationService notificationService,
           IHttpContextAccessor httpContextAccessor
           )

        {
            _context = context;
            _propertyService = propertyService;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
        }

        public async Task<Result> CreatePriceSuggestion(PriceSuggestion priceSuggestion)
        {

            var httpContext = _httpContextAccessor.HttpContext;
            var userDto = (Task<UserDto>)httpContext!.Items["User"];
            var userId = userDto.Result.Id;
            var property = _propertyService.GetPropertyById(priceSuggestion.PropertyId).Result;
            if (property is null)
            {
                return Result.Failure(PriceSuggestionErrors.InvalidPropertyId(priceSuggestion.PropertyId));
            }

            if (GetPriceSuggestionByPropertyIdByUserId(priceSuggestion.PropertyId, userId).Result != null)
            {
                return Result.Failure(PriceSuggestionErrors.UserAlreadyCreatedPriceSuggestion);
            }

            priceSuggestion.Created = DateTime.Now;
            priceSuggestion.UserId = userId;

            await _context.PriceSuggestions.InsertOneAsync(priceSuggestion);

            var notification = new Notification(false, priceSuggestion.Id, priceSuggestion.PropertyId, userId, property.UserId, NotificationTypes.AboveAsking)
            {
                Created = DateTime.Now
            };
            await _notificationService.CreateNotification(notification);
            return Result.Success();
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

        public async Task<PriceSuggestionResponse> GetPriceSuggestions(string propertyId, int page, int pageSize)
        {
            Expression<Func<PriceSuggestion, bool>> expression;
            if (!string.IsNullOrEmpty(propertyId))
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
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .Unwind("Property")
                .As<PriceSuggestion>()
                .ToListAsync();

            return new PriceSuggestionResponse(new PaginationMetadata((int)totalRecordsCount, currentPage: page, (int)Math.Ceiling(totalRecordsCount / (double)pageSize)), priceSuggestions);
        }


        private void ValidatePropertyId(string? propertyId)
        {
            if (_propertyService.GetPropertyById(propertyId).Result == null)
            {
                throw new CustomException("Invalid property ID");
            }
        }

        public async Task<PriceSuggestionsStatisticsResponse> GetPriceSuggestionsStatistics()
        {
            var aboveAskingCount = await GetPriceSuggestionsStatisticsByExpression(x => x.DifferenceInPercentage > 0);
            var atAskingCount = await GetPriceSuggestionsStatisticsByExpression(x => x.DifferenceInPercentage == 0);
            var belowAskingCount = await GetPriceSuggestionsStatisticsByExpression(x => x.DifferenceInPercentage < 0);

            return new PriceSuggestionsStatisticsResponse(
                (int)aboveAskingCount,
                (int)atAskingCount,
                (int)belowAskingCount
                );
        }

        private async Task<long> GetPriceSuggestionsStatisticsByExpression(Expression<Func<PriceSuggestion, bool>> expression)
        {
            return await _context.PriceSuggestions.CountDocumentsAsync(Builders<PriceSuggestion>.Filter.Where(expression));
        }
    }
}

