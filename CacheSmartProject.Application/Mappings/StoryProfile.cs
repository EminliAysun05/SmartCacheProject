using AutoMapper;
using CacheSmartProject.Domain.Dtos.Story;
using CacheSmartProject.Domain.Entities;


namespace CacheSmartProject.Application.Mappings
{
    public class StoryProfile : Profile
    {
        public StoryProfile()
        {
            CreateMap<Story,StoryResponseDto>().ReverseMap();
            CreateMap<Story, StoryCreateDto>().ReverseMap();
            CreateMap<Story, StoryUpdateDto>().ReverseMap();
        }
    }
}
