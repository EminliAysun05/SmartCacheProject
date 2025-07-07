using AutoMapper;
using CacheSmartProject.Domain.Dtos.Category;
using CacheSmartProject.Domain.Entities;


namespace CacheSmartProject.Application.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryResponseDto>().ReverseMap();
        CreateMap<Category, CategoryCreateDto>().ReverseMap();
        CreateMap<Category, CategoryUpdateDto>().ReverseMap();
    }
}
