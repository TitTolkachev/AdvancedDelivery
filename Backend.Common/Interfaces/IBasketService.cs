using Backend.Common.Dto;

namespace Backend.Common.Interfaces;

public interface IBasketService
{
    Task<List<DishBasketDto>> GetUserCart(Guid userId);
    Task AddDishToCart(Guid dishId, Guid userId);
    Task RemoveDishFromCart(Guid dishId, Guid userId);
}