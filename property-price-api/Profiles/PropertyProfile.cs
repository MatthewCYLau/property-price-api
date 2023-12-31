﻿using AutoMapper;
using property_price_api.Models;

namespace property_price_api.Profiles
{
	public class PropertyProfile : Profile
    {
		public PropertyProfile()
		{

            CreateMap<CreatePropertyRequest, Property>()
               .ForMember(dest => dest.ListingUrl, opt => opt.MapFrom(src => src.ListingUrl))
               .ForMember(dest => dest.AskingPrice, opt => opt.MapFrom(src => src.AskingPrice))
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
               .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => "")
               );

            CreateMap<Property, CreatePropertyResponse>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.ListingUrl, opt => opt.MapFrom(src => src.ListingUrl))
               .ForMember(dest => dest.AskingPrice, opt => opt.MapFrom(src => src.AskingPrice))
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
               .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
               .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl)
               );

            CreateMap<Property, PropertyDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.ListingUrl, opt => opt.MapFrom(src => src.ListingUrl))
                .ForMember(dest => dest.AskingPrice, opt => opt.MapFrom(src => src.AskingPrice))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.UserDto, opt => opt.MapFrom(src => new UserDto(
                    src.User.Id,
                    src.User.Email,
                    src.User.UserType))
                );
        }
	}
}

