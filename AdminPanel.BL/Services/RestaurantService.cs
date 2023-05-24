using AdminPanel.Common.Interfaces;
using AdminPanel.Common.Models.Restaurant;
using Backend.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.BL.Services;

public class RestaurantService : IRestaurantService
{
    private readonly ApplicationDbContext _context;

    public RestaurantService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateRestaurant(Restaurant restaurant)
    {
        var newRest = new Backend.DAL.Entities.Restaurant
        {
            Id = new Guid(),
            Name = restaurant.Name
        };

        if (await _context.Restaurants.AnyAsync(r => r.Name == restaurant.Name))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Duplicate restaurant name"
            );
            throw ex;
        }

        await _context.Restaurants.AddAsync(newRest);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Restaurant>> GetRestaurants()
    {
        var rests = await _context
            .Restaurants
            .Select(r => new Restaurant { Id = r.Id, Name = r.Name })
            .ToListAsync();

        return rests;
    }

    public async Task DeleteRestaurant(Guid id)
    {
        var restaurant = await _context
            .Restaurants
            .FirstOrDefaultAsync(r => r.Id == id);
        if (restaurant == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Restaurant was not found"
            );
            throw ex;
        }

        _context.Remove(restaurant);
        await _context.SaveChangesAsync();
    }

    public async Task ChangeRestaurant(Restaurant restaurant)
    {
        if (await _context.Restaurants.AnyAsync(r => r.Name == restaurant.Name && r.Id != restaurant.Id))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                $"Restaurant with name {restaurant.Name} already exists"
            );
            throw ex;
        }

        var rest = await _context
            .Restaurants
            .FirstOrDefaultAsync(r => r.Id == restaurant.Id);
        if (rest == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Restaurant was not found"
            );
            throw ex;
        }

        rest.Name = restaurant.Name;

        await _context.SaveChangesAsync();
    }

    public async Task<Restaurant> GetRestInfo(Guid id)
    {
        var rest = await _context
            .Restaurants
            .FirstOrDefaultAsync(r => r.Id == id);
        if (rest == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Restaurant was not found"
            );
            throw ex;
        }

        var restInfo = new Restaurant
        {
            Id = rest.Id,
            Name = rest.Name
        };
        return restInfo;
    }
}