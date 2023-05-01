using AdminPanel.Common.Interfaces;
using AdminPanel.Common.Models;

namespace AdminPanel.BL.Services;

public class RestaurantService : IRestaurantService
{
    public Task<List<Restaurant>> GetRestaurants()
    {
        var list = new List<Restaurant>
        {
            new(),
            new(),
            new()
        };
        return Task.FromResult(list);
    }
}