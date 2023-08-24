using AutoMapper;
using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
	public class PropertyService
	{

        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;


        public PropertyService(MongoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PropertyDto>> GetAsync()
        {
            var properties = await _context.Properties.Aggregate()
            .Lookup("users", "UserId", "_id", @as: "User")
            .Unwind("User")
            .As<Property>()
            .ToListAsync();

            var propertiesDto = _mapper.Map<List<PropertyDto>>(properties);
            return propertiesDto;
        }
       
        public async Task<PropertyDto?> GetAsync(string id)
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

        public async Task<PropertyDto> CreateAsync(CreatePropertyDto createPropertyDto)
        {
            var _property = _mapper.Map<Property>(createPropertyDto);
            await _context.Properties.InsertOneAsync(_property);
            var _createdProperty = _mapper.Map<PropertyDto>(_property);

            return _createdProperty;
        }
            

        public async Task UpdateAsync(string id, Property property) =>
            await _context.Properties.ReplaceOneAsync(x => x.Id == id, property);

        public async Task RemoveAsync(string id) =>
            await _context.Properties.DeleteOneAsync(x => x.Id == id);
    }
}

