using AutoMapper;
using Backend.Common.Dto;
using Backend.DAL.Entities;

namespace DeliveryBackend.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrderInfoDto>();
        CreateMap<Dish, DishDto>().ReverseMap();
        CreateMap<UserRegisterModel, User>();
        CreateMap<User, UserDto>();
    }
}