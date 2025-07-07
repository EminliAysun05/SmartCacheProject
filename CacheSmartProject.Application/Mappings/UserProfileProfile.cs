using AutoMapper;
using CacheSmartProject.Domain.Dtos.UserProfile;

namespace CacheSmartProject.Application.Mappings;

public class UserProfileProfile : Profile
{
    public UserProfileProfile()
    {
        CreateMap<UserProfileProfile, UserProfileResponseDto>().ReverseMap();
        CreateMap<UserProfileProfile, UserProfileCreateDto>().ReverseMap();
        CreateMap<UserProfileProfile, UserProfileUpdateDto>().ReverseMap();
    }
}

