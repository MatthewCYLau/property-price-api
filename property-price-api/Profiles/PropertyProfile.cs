using AutoMapper;
using property_price_api.Models;

namespace property_price_api.Profiles
{
	public class PropertyProfile : Profile
    {
		public PropertyProfile()
		{
            _ = CreateMap<Property, PropertyDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ListingUrl, opt => opt.MapFrom(src => src.ListingUrl))
                .ForMember(dest => dest.AskingPrice, opt => opt.MapFrom(src => src.AskingPrice))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserDto, opt => opt.MapFrom(src => new UserDto(src.User.Id, src.User.Email))
                );
        }
	}
}

