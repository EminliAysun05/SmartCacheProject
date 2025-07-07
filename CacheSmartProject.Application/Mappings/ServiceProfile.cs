using AutoMapper;
using CacheSmartProject.Domain.Dtos.Service;
using CacheSmartProject.Domain.Entities;


namespace CacheSmartProject.Application.Mappings;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<Service, ServiceResponseDto>().ReverseMap();
        CreateMap<Service, ServiceCreateDto>().ReverseMap();
        CreateMap<Service, ServiceUpdateDto>().ReverseMap();
    }
}
