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

public class DishService : IDishService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DishService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DishPagedListDto> GetDishList(GetDishListQuery dishListQuery)
    {
        var dishList = await GetDishesByDishListQuery(dishListQuery);

        var dishesOrdered = OrderDishes(dishListQuery, dishList);

        var dishes = dishesOrdered.Skip((dishListQuery.Page - 1) * 5).Take(Range.EndAt(5)).ToList();

        var pagination = new PageInfoModel
        {
            Size = dishes.Count,
            Count = (dishList.Count + 4) / 5,
            Current = dishListQuery.Page
        };

        if (pagination.Current <= pagination.Count && pagination.Current > 0)
            return new DishPagedListDto
            {
                Dishes = _mapper.Map<List<DishDto>>(dishes),
                Pagination = pagination
            };

        var ex = new Exception();
        ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
            "Invalid value for attribute page"
        );
        throw ex;
    }

    public async Task<DishDto> GetDish(Guid id)
    {
        var dishEntity = await _context.Dishes
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (dishEntity != null)
            return _mapper.Map<DishDto>(dishEntity);

        var ex = new Exception();
        ex.Data.Add(StatusCodes.Status404NotFound.ToString(),
            "Dish entity not found"
        );
        throw ex;
    }

    public async Task<bool> CheckDishRating(Guid id, Guid userId)
    {
        await CheckDishInDb(id);

        var ratingEntity = await _context.Ratings.FirstOrDefaultAsync(x => x.DishId == id && x.UserId == userId);
        return ratingEntity == null && await IsDishOrdered(id, userId);
    }

    public async Task SetDishRating(Guid id, int rating, Guid userId)
    {
        CheckRating(rating);
        await CheckDishInDb(id);
        if(!await IsDishOrdered(id, userId))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "User can't set rating on dish that wasn't ordered"
            );
            throw ex;
        }
        if (await CheckDishRating(id, userId))
        {
            _context.Ratings.Add(new Rating
            {
                Id = Guid.NewGuid(),
                DishId = id,
                UserId = userId,
                RatingScore = rating
            });

            await _context.SaveChangesAsync();

            var dishEntity = await _context.Dishes.FirstOrDefaultAsync(x => x.Id == id);
            var dishRatingList = await _context.Ratings.Where(x => x.DishId == id).ToListAsync();
            var sum = dishRatingList.Sum(r => r.RatingScore);
            dishEntity!.Rating = (double) sum / dishRatingList.Count;

            await _context.SaveChangesAsync();
        }
        else
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
                "Rating entity already exists"
            );
            throw ex;
        }
    }

    private async Task CheckDishInDb(Guid dishId)
    {
        if (await _context.Dishes.FirstOrDefaultAsync(x => x.Id == dishId) == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status404NotFound.ToString(),
                "Dish entity not found"
            );
            throw ex;
        }
    }

    private static void CheckRating(int rating)
    {
        if (rating is >= 0 and <= 10) return;
        var e = new Exception();
        e.Data.Add(StatusCodes.Status400BadRequest.ToString(),
            "Bad Request, Rating range is 0-10"
        );
        throw e;
    }

    private async Task<bool> IsDishOrdered(Guid id, Guid userId)
    {
        var carts = await _context.Carts.Where(x => x.DishId == id
        && x.UserId == userId && x.OrderId != null).ToListAsync();
        
        foreach (var cart in carts)
        {
            if (await _context.Orders.FirstOrDefaultAsync(x=>
                    x.UserId == userId && x.Id == cart.OrderId && 
                    x.Status == OrderStatus.Delivered.ToString()) != null)
                return true;
        }

        return false;
    }

    private static IEnumerable<Dish> OrderDishes(GetDishListQuery dishListQuery, IEnumerable<Dish> dishList)
    {
        var orderBy = dishListQuery.Sorting;
        if (orderBy == DishSorting.NameAsc.ToString())
            return dishList.OrderBy(s => s.Name).ToList();
        if (orderBy == DishSorting.NameDesc.ToString())
            return dishList.OrderByDescending(s => s.Name).ToList();
        if (orderBy == DishSorting.PriceAsc.ToString())
            return dishList.OrderBy(s => s.Price).ToList();
        if (orderBy == DishSorting.PriceDesc.ToString())
            return dishList.OrderByDescending(s => s.Price).ToList();
        if (orderBy == DishSorting.RatingAsc.ToString())
            return dishList.OrderBy(s => s.Rating).ToList();
        return orderBy == DishSorting.RatingDesc.ToString()
            ? dishList.OrderByDescending(s => s.Rating).ToList()
            : dishList.OrderBy(s => s.Name).ToList();
    }

    private async Task<List<Dish>> GetDishesByDishListQuery(GetDishListQuery dishListQuery)
    {
        foreach (var category in dishListQuery.Categories)
        {
            if (category != DishCategory.Dessert.ToString()
                && category != DishCategory.Drink.ToString()
                && category != DishCategory.Soup.ToString()
                && category != DishCategory.Wok.ToString()
                && category != DishCategory.Pizza.ToString()
                && !category.IsNullOrEmpty())
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                    $"Dish Category {category} is not available"
                );
                throw ex;
            }

            if (category.IsNullOrEmpty())
            {
                if (dishListQuery.Vegetarian == null)
                    return await _context.Dishes.ToListAsync();
                return await _context.Dishes.Where(x =>
                    dishListQuery.Vegetarian == x.Vegetarian).ToListAsync();
            }
        }

        if (dishListQuery.Categories.IsNullOrEmpty())
        {
            if (dishListQuery.Vegetarian == null)
                return await _context.Dishes.ToListAsync();
            return await _context.Dishes.Where(x =>
                dishListQuery.Vegetarian == x.Vegetarian).ToListAsync();
        }

        if (dishListQuery.Vegetarian == null)
            return await _context.Dishes.Where(x =>
                dishListQuery.Categories.Contains(x.Category)).ToListAsync();
        return await _context.Dishes.Where(x =>
            dishListQuery.Categories.Contains(x.Category) &&
            dishListQuery.Vegetarian == x.Vegetarian).ToListAsync();
    }

    // --------------------------------------------------------------
    // --------------------------------------------------------------
    public async Task AddDishes(List<DishDto> dishes)
    {
        _context.Dishes.AddRange(_mapper.Map<List<Dish>>(dishes));
        await _context.SaveChangesAsync();
    }
}