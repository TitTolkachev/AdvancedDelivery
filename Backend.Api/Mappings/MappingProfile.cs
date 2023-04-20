using AutoMapper;
using DeliveryBackend.Data.Models.Entities;
using DeliveryBackend.DTO;

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