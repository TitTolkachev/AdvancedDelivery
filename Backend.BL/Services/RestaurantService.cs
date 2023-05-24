using Backend.Common.Dto;
using Backend.Common.Dto.Queries;
using Backend.Common.Interfaces;
using Backend.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Backend.BL.Services;

public class RestaurantService : IRestaurantService
{
    private const int PageSize = 10;

    private readonly ApplicationDbContext _context;

    public RestaurantService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RestaurantPagedListDto> GetRestaurantList(GetRestaurantListQuery query)
    {
        var restaurants = await _context
            .Restaurants
            .Where(r => query.SearchName == null || r.Name.Contains(query.SearchName))
            .Include(r => r.Menus)
            .ThenInclude(menu => menu.Dishes)
            .ToListAsync();
        if (restaurants == null || query.Page > (restaurants.Count + PageSize - 1) / PageSize)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Restaurants were not found");
            throw ex;
        }

        var restaurantsDto = restaurants
            .Skip((query.Page - 1) * PageSize)
            .Take(Range.EndAt(PageSize))
            .ToList()
            .Select(r =>
                new RestaurantDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Menus = new List<MenuDto>(r.Menus.Select(m => new MenuDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Dishes = m.Dishes.Select(d => d.Id).ToList()
                    }).ToList())
                }).ToList();

        return new RestaurantPagedListDto
        {
            Restaurants = restaurantsDto,
            Pagination = new PageInfoModel
            {
                Size = restaurantsDto.Count,
                Count = (restaurants.Count + PageSize - 1) / PageSize,
                Current = query.Page
            }
        };
    }

    public async Task<RestaurantDto> GetRestaurant(Guid restaurantId)
    {
        var restaurant = await _context
            .Restaurants
            .Where(r => r.Id == restaurantId)
            .Include(r => r.Menus)
            .ThenInclude(menu => menu.Dishes)
            .FirstOrDefaultAsync();
        if (restaurant == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Restaurant was not found");
            throw ex;
        }

        var restaurantDto = new RestaurantDto
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            Menus = new List<MenuDto>(restaurant.Menus.Select(m => new MenuDto
            {
                Id = m.Id,
                Name = m.Name,
                Dishes = m.Dishes.Select(d => d.Id).ToList()
            }).ToList())
        };

        return restaurantDto;
    }
}