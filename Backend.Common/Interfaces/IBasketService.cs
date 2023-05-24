using Backend.Common.Dto;

namespace Backend.Common.Interfaces;

public interface IBasketService
{
    Task<List<DishBasketDto>> GetUserCart(UserInfoDto userInfoDto, Guid userId);
    Task AddDishToCart(UserInfoDto userInfoDto, Guid dishId, Guid userId);
    Task RemoveDishFromCart(UserInfoDto userInfoDto, Guid dishId, Guid userId);
}