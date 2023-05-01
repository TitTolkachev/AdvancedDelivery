using AdminPanel.Common.Models;

namespace AdminPanel.Common.Interfaces;

public interface IRestaurantService
{
    Task<List<Restaurant>> GetRestaurants();
}