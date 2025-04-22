using System;
using AutoMapper;
using BookSphere.DTOs;
using BookSphere.Models;

namespace BookSphere.Mapping;

public class MappingProfile : Profile
{
          public MappingProfile()
          {
                    //User Mappings
                    CreateMap<User, UserDto>()
                              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress));
                    CreateMap<RegisterDto, User>()
                              .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
                              .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                              .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Member"))
                              .ForMember(dest => dest.RegisteredDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                              .ForMember(dest => dest.SuccessfulOrder, opt => opt.MapFrom(src => 0))
                              .ForMember(dest => dest.HasStackableDiscount, opt => opt.MapFrom(src => false));
          }
}
