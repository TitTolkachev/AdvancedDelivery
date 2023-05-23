using Backend.Common.Dto;
using Backend.Common.Dto.Queries;

namespace Backend.Common.Interfaces;

public interface IRestaurantService
{
    public Task<RestaurantPagedListDto> GetRestaurantList(GetRestaurantListQuery query);

    public Task<RestaurantDto> GetRestaurant(Guid restaurantId);
}