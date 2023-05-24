using AutoMapper;
using Backend.BL.Enums;
using Backend.Common.Dto;
using Backend.Common.Dto.Queries;
using Backend.Common.Interfaces;
using Backend.DAL;
using Backend.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.BL.Services;

public class ManagerService : IManagerService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    private const int PageSize = 10;

    public ManagerService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrderPagedListDto> GetOrders(GetOrdersManagerListQuery query, Guid managerId)
    {
        await InitManager(managerId);

        var orders = _context.Orders.Where(x =>
            (query.Statuses.IsNullOrEmpty() || query.Statuses!.Contains(x.Status)) &&
            x.Number.ToString().Contains(query.SearchOrderNumber ?? string.Empty)
        );

        var selectedOrders = await orders
            .Skip((query.Page - 1) * PageSize)
            .Take(Range.EndAt(PageSize))
            .ToListAsync();

        var pagination = new PageInfoModel
        {
            Size = selectedOrders.Count,
            Count = (orders.Count() + PageSize - 1) / PageSize,
            Current = query.Page
        };

        if (pagination.Current <= pagination.Count && pagination.Current > 0)
        {
            return new OrderPagedListDto
            {
                Orders = SortOrders(_mapper.Map<List<OrderInfoDto>>(selectedOrders), query).ToList(),
                Pagination = pagination
            };
        }

        var ex = new Exception();
        ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
            "Invalid value for attribute page"
        );
        throw ex;
    }

    public async Task CreateMenu(CreateMenuDto createMenuDto, Guid managerId)
    {
        var manager = await InitManager(managerId);

        if (await _context.Menus.AnyAsync(menu => menu.Name == createMenuDto.Name))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Duplicate menu name"
            );
            throw ex;
        }

        var restaurantEntity = await _context
            .Restaurants
            .Include(r => r.Managers)
            .Include(r => r.Menus)
            .FirstOrDefaultAsync(r => r.Id == createMenuDto.RestaurantId);
        if (restaurantEntity == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Restaurant was not found"
            );
            throw ex;
        }

        if (!restaurantEntity.Managers.Contains(manager))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                $"Manager with id \"{managerId}\" does not work in restaurant with id \"{createMenuDto.RestaurantId}\""
            );
            throw ex;
        }

        var menuEntity = new Menu
        {
            Id = new Guid(),
            Name = createMenuDto.Name,
            RestaurantId = restaurantEntity.Id,
            Restaurant = restaurantEntity
        };

        restaurantEntity.Managers.Add(manager);
        await _context.Menus.AddAsync(menuEntity);
        await _context.SaveChangesAsync();
    }

    public async Task ChangeMenu(Guid menuId, ChangeMenuDto changeMenuDto, Guid managerId)
    {
        await InitManager(managerId);

        if (await _context.Menus.AnyAsync(menu => menu.Name == changeMenuDto.Name))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Duplicate menu name"
            );
            throw ex;
        }

        var menuEntity = await _context
            .Menus
            .Include(m => m.Restaurant)
            .ThenInclude(r => r.Managers)
            .FirstOrDefaultAsync(menu => menu.Id == menuId);
        if (menuEntity == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Menu was not found"
            );
            throw ex;
        }

        if (menuEntity.Restaurant.Managers.All(m => m.Id != managerId))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                $"Manager with id \"{managerId}\" does not work in restaurant with id \"{changeMenuDto.RestaurantId}\""
            );
            throw ex;
        }

        menuEntity.Name = changeMenuDto.Name;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteMenu(Guid menuId, Guid managerId)
    {
        await InitManager(managerId);

        var menuEntity = await _context
            .Menus
            .Include(m => m.Restaurant)
            .ThenInclude(r => r.Managers)
            .Include(m => m.Dishes)
            .FirstOrDefaultAsync(menu => menu.Id == menuId);
        if (menuEntity == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Menu was not found"
            );
            throw ex;
        }

        if (menuEntity.Restaurant.Managers.All(m => m.Id != managerId))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                $"Manager with id \"{managerId}\" does not work in restaurant with id \"{menuEntity.RestaurantId}\""
            );
        }

        menuEntity.Dishes = new List<Dish>();
        _context.Menus.Remove(menuEntity);
        await _context.SaveChangesAsync();
    }

    public async Task AddDishToMenu(Guid menuId, Guid dishId, Guid managerId)
    {
        await InitManager(managerId);

        var menuEntity = await _context
            .Menus
            .Include(m => m.Restaurant)
            .ThenInclude(r => r.Managers)
            .FirstOrDefaultAsync(menu => menu.Id == menuId);
        if (menuEntity == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Menu was not found"
            );
            throw ex;
        }

        var dishEntity = await _context
            .Dishes
            .FirstOrDefaultAsync(dish => dish.Id == dishId);
        if (dishEntity == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Dish was not found"
            );
            throw ex;
        }

        if (menuEntity.Restaurant.Managers.All(m => m.Id != managerId))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                $"Manager with id \"{managerId}\" does not work in restaurant with id \"{menuEntity.RestaurantId}\""
            );
        }

        if (menuEntity.Dishes.Contains(dishEntity))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Dish already was in menu"
            );
            throw ex;
        }

        menuEntity.Dishes.Add(dishEntity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveDishFromMenu(Guid menuId, Guid dishId, Guid managerId)
    {
        await InitManager(managerId);

        var menuEntity = await _context
            .Menus
            .Include(m => m.Restaurant)
            .ThenInclude(r => r.Managers)
            .FirstOrDefaultAsync(menu => menu.Id == menuId);
        if (menuEntity == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Menu was not found"
            );
            throw ex;
        }

        var dishEntity = await _context
            .Dishes
            .FirstOrDefaultAsync(dish => dish.Id == dishId);
        if (dishEntity == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Dish was not found"
            );
            throw ex;
        }

        if (menuEntity.Restaurant.Managers.All(m => m.Id != managerId))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                $"Manager with id \"{managerId}\" does not work in restaurant with id \"{menuEntity.RestaurantId}\""
            );
        }

        if (!menuEntity.Dishes.Contains(dishEntity))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Dish in menu was not found"
            );
            throw ex;
        }

        menuEntity.Dishes.Remove(dishEntity);
        await _context.SaveChangesAsync();
    }

    private static IEnumerable<OrderInfoDto> SortOrders(IEnumerable<OrderInfoDto> orders,
        GetOrdersManagerListQuery query)
    {
        var orderBy = query.Sorting;
        if (orderBy == CookOrderSorting.CookedDateAsc.ToString())
            return orders.OrderBy(s => s.DeliveryTime).ToList();
        if (orderBy == CookOrderSorting.CookedDateDesc.ToString())
            return orders.OrderByDescending(s => s.DeliveryTime).ToList();
        return (orderBy == CookOrderSorting.CreateDateAsc.ToString())
            ? orders.OrderBy(s => s.OrderTime).ToList()
            : orders.OrderByDescending(s => s.OrderTime).ToList();
    }

    private async Task<Manager> InitManager(Guid managerId)
    {
        var manager = await _context
            .Managers
            .FirstOrDefaultAsync(c => c.Id == managerId);
        if (manager != null) return manager;

        var newManager = new Manager
        {
            Id = managerId
        };

        await _context.Managers.AddAsync(newManager);
        await _context.SaveChangesAsync();
        manager = newManager;

        return manager;
    }
}