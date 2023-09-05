using AutoMapper;
using property_price_api.Models;

namespace property_price_api.Profiles
{
	public class UserProfile : Profile
    {
		public UserProfile()
		{
			CreateMap<CreateUserRequest, User>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType)
                );

			CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType)
                );

        }
	}
}

