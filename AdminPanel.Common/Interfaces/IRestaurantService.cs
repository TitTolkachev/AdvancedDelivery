using AdminPanel.Common.Models;

namespace AdminPanel.Common.Interfaces;

public interface IRestaurantService
{
    public Task CreateRestaurant(Restaurant restaurant);
    public Task<List<Restaurant>> GetRestaurants();
    public Task DeleteRestaurant(Guid id);
    public Task ChangeRestaurant(Restaurant restaurant);
    public Task<Restaurant> GetRestInfo(Guid id);
}