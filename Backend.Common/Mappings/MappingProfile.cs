using AutoMapper;
using Backend.Common.Dto;
using Backend.DAL.Entities;

namespace Backend.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrderInfoDto>();
        CreateMap<Dish, DishDto>().ReverseMap();
    }
}