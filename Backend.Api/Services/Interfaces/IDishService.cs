using DeliveryBackend.DTO;
using DeliveryBackend.DTO.Queries;

namespace DeliveryBackend.Services.Interfaces;

public interface IDishService
{
    Task<DishPagedListDto> GetDishList(GetDishListQuery dishListQuery);
    Task<DishDto> GetDish(Guid dishId);
    Task<bool> CheckDishRating(Guid id, Guid userId);
    Task SetDishRating(Guid id, int rating, Guid userId);
    
    // --------------------
    // --------------------
    Task AddDishes(List<DishDto> dishes);
}