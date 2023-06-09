﻿using AutoMapper;
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

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    private const int PageSize = 10;

    public OrderService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrderDto> GetOrderInfo(UserInfoDto userInfoDto, Guid userId, Guid orderId)
    {
        await InitUser(userInfoDto);
        
        var orderInfo = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (orderInfo == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Order Info not found"
            );
            throw ex;
        }

        if (orderInfo.UserId != userId)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                "Invalid order owner"
            );
            throw ex;
        }

        var orderCarts = await _context.Carts.Where(x => x.OrderId == orderId).ToListAsync();
        if (orderCarts.IsNullOrEmpty())
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Dishes in Order not found"
            );
            throw ex;
        }

        var dishes = new List<Dish>();
        foreach (var orderCart in orderCarts)
        {
            var dish = await _context.Dishes.FirstOrDefaultAsync(x => x.Id == orderCart.DishId);

            if (dish != null)
                dishes.Add(dish);
            else
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                    "Dish in Order not found"
                );
                throw ex;
            }
        }

        var convertedDishes = (from orderCart in orderCarts
            let dish = dishes.FirstOrDefault(x => x.Id == orderCart.DishId)
            where dish != null
            select new DishBasketDto
            {
                Id = dish.Id,
                Name = dish.Name,
                Price = dish.Price,
                TotalPrice = orderCart.Amount * dish.Price,
                Amount = orderCart.Amount,
                Image = dish.Image
            }).ToList();

        if (!convertedDishes.IsNullOrEmpty())
            return new OrderDto
            {
                Id = orderInfo.Id,
                DeliveryTime = orderInfo.DeliveryTime,
                OrderTime = orderInfo.OrderTime,
                Status = orderInfo.Status,
                Price = orderInfo.Price,
                Dishes = convertedDishes,
                Address = orderInfo.Address
            };
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Empty order list returned"
            );
            throw ex;
        }
    }

    public async Task<OrderPagedListDto> GetOrders(UserInfoDto userInfoDto, Guid userId, GetOrdersListQuery query)
    {
        await InitUser(userInfoDto);
        
        var orders = _context.Orders.Where(x =>
            x.UserId == userId &&
            x.Number.ToString().Contains(query.SearchOrderNumber ?? string.Empty) &&
            x.OrderTime >= (query.DateStart ?? DateTime.MinValue) &&
            x.DeliveryTime <= (query.DateEnd ?? DateTime.MaxValue)
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
                Orders = _mapper.Map<List<OrderInfoDto>>(selectedOrders),
                Pagination = pagination
            };
        }

        var ex = new Exception();
        ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
            "Invalid value for attribute page"
        );
        throw ex;
    }

    public async Task CreateOrder(UserInfoDto userInfoDto, Guid userId, OrderCreateDto orderCreateDto)
    {
        await InitUser(userInfoDto);
        
        if (orderCreateDto.DeliveryTime - DateTime.Now < TimeSpan.FromMinutes(5) ||
            orderCreateDto.DeliveryTime - DateTime.Now > TimeSpan.FromHours(24))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Bad request, Delivery time range is 5m - 24h"
            );
            throw ex;
        }

        var cartDishes = await _context.Carts
            .Where(x => x.UserId == userId && x.OrderId == null)
            .ToListAsync();
        if (cartDishes.IsNullOrEmpty())
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Dishes in cart were not found"
            );
            throw ex;
        }

        var orderId = Guid.NewGuid();
        var newOrder = new Order
        {
            Id = orderId,
            Number = await _context.Orders.CountAsync() + 1,
            DeliveryTime = orderCreateDto.DeliveryTime,
            OrderTime = DateTime.UtcNow,
            Status = OrderStatus.Created.ToString(),
            Price = 0,
            Address = orderCreateDto.Address,
            UserId = userId
        };

        // Проверка, что все блюда из одного ресторана
        await CheckDishes(cartDishes, orderCreateDto.RestaurantId);

        await _context.Orders.AddAsync(newOrder);
        await _context.SaveChangesAsync();
        newOrder.Price = await CreateOrderOperations(orderId, cartDishes);
        await _context.SaveChangesAsync();
    }

    public async Task RepeatOrder(UserInfoDto userInfoDto, Guid userId, OrderRepeatDto orderRepeatDto)
    {
        await InitUser(userInfoDto);
        
        if (orderRepeatDto.DeliveryTime - DateTime.Now < TimeSpan.FromMinutes(5) ||
            orderRepeatDto.DeliveryTime - DateTime.Now > TimeSpan.FromHours(24))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Bad request, Delivery time range is 5m - 24h"
            );
            throw ex;
        }

        var previousOrderCarts = await _context.Carts
            .Where(x => x.UserId == userId && x.OrderId == orderRepeatDto.OrderId)
            .ToListAsync();
        if (previousOrderCarts.IsNullOrEmpty())
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Dishes were not found"
            );
            throw ex;
        }

        var orderId = Guid.NewGuid();
        var newOrder = new Order
        {
            Id = orderId,
            Number = await _context.Orders.CountAsync() + 1,
            DeliveryTime = orderRepeatDto.DeliveryTime,
            OrderTime = DateTime.UtcNow,
            Status = OrderStatus.Created.ToString(),
            Price = 0,
            Address = orderRepeatDto.Address,
            UserId = userId
        };

        // Проверка, что все блюда из одного ресторана
        await CheckDishes(previousOrderCarts, orderRepeatDto.RestaurantId);

        await _context.Orders.AddAsync(newOrder);
        await _context.SaveChangesAsync();
        newOrder.Price = await RepeatOrderOperations(userId, orderId, previousOrderCarts);
        await _context.SaveChangesAsync();
    }

    public async Task ConfirmOrderDelivery(UserInfoDto userInfoDto, Guid userId, Guid orderId)
    {
        await InitUser(userInfoDto);
        
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);

        if (order == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Order Info not found"
            );
            throw ex;
        }

        if (order.UserId != userId)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                "Invalid order owner"
            );
            throw ex;
        }

        order.Status = OrderStatus.Delivered.ToString();
        await _context.SaveChangesAsync();
    }

    private async Task<decimal> CreateOrderOperations(Guid orderId, IReadOnlyList<Cart> cartDishes)
    {
        decimal res = 0;

        for (var i = 0; i < cartDishes.Count; i++)
        {
            cartDishes[i].OrderId = orderId;
            var dish = await _context.Dishes.FirstOrDefaultAsync(x => x.Id == cartDishes[i].DishId);
            if (dish == null)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                    "Dish in Order not found"
                );
                throw ex;
            }

            res += cartDishes[i].Amount * dish.Price;
        }

        await _context.SaveChangesAsync();

        return res;
    }

    private async Task<decimal> RepeatOrderOperations(Guid userId, Guid orderId, IEnumerable<Cart> previousOrderCarts)
    {
        decimal res = 0;

        foreach (var cart in previousOrderCarts)
        {
            res += cart.Amount * (await _context.Dishes.FirstOrDefaultAsync(d => d.Id == cart.DishId))!.Price;
            await _context.Carts.AddAsync(new Cart
            {
                Id = Guid.NewGuid(),
                DishId = cart.DishId,
                Amount = cart.Amount,
                UserId = userId,
                OrderId = orderId
            });
        }

        await _context.SaveChangesAsync();

        return res;
    }

    private async Task CheckDishes(List<Cart> cartDishes, Guid restaurantId)
    {
        var menus = await _context.Menus.Where(m => m.Restaurant.Id == restaurantId).ToListAsync();

        foreach (var cartDish in cartDishes)
        {
            if (menus.Any(m => m.Dishes.Any(d => d.Id == cartDish.DishId))) continue;
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                $"Dish with id \"{cartDish.DishId}\" can not be delivered from restaurant with id \"{restaurantId}\""
            );
            throw ex;
        }
    }

    private async Task InitUser(UserInfoDto userInfoDto)
    {
        var customer = await _context
            .Users
            .FirstOrDefaultAsync(c => c.Id == userInfoDto.id);
        if (customer == null)
        {
            var newCustomer = new User
            {
                Id = userInfoDto.id,
                Address = userInfoDto.address
            };

            await _context.Users.AddAsync(newCustomer);
            await _context.SaveChangesAsync();
            customer = newCustomer;
        }

        customer.Address = userInfoDto.address ?? customer.Address;
    }
}