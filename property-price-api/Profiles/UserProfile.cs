using AutoMapper;
using property_price_api.Models;

namespace property_price_api.Profiles
{
	public class UserProfile : Profile
    {
		public UserProfile()
		{
			CreateMap<CreateUserDto, User>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password)
                );

			CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)
                );

        }
	}
}

