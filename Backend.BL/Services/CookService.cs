using AutoMapper;
using Backend.BL.Enums;
using Backend.Common.Dto;
using Backend.Common.Dto.Queries;
using Backend.Common.Interfaces;
using Backend.DAL;
using Backend.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Backend.BL.Services;

public class CookService : ICookService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProducerService _producerService;

    private const int PageSize = 10;

    public CookService(ApplicationDbContext context, IProducerService producerService, IMapper mapper)
    {   
        _context = context;
        _producerService = producerService;
        _mapper = mapper;
    }

    public async Task<OrderPagedListDto> GetFreeOrders(GetOrdersCookListQuery query, Guid cookId)
    {
        var cook = await InitCook(cookId);
        var orders = _context.Orders.Where(x =>
            x.RestaurantId == cook.RestaurantId &&
            x.Status == OrderStatus.Created.ToString()
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

    public async Task<OrderPagedListDto> GetCookedOrders(GetOrdersListQuery query, Guid cookId)
    {
        var orders = _context.Orders.Where(x =>
            x.CookId == cookId &&
            x.Status != OrderStatus.Created.ToString() &&
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

    public async Task SetOrderStatusPackaging(Guid orderId, Guid cookId)
    {
        var order = await _context
            .Orders
            .Include(order => order.Cook)
            .Include(order => order.User)
            .FirstOrDefaultAsync(order => order.Id == orderId);
        if (order == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                $"Order with id \"{orderId}\" was not found"
            );
            throw ex;
        }

        if (order.CookId != cookId)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                $"Cook with id \"{cookId}\" does not work with order \"{orderId}\""
            );
            throw ex;
        }

        if (order.Status != OrderStatus.Kitchen.ToString())
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                $"Cook can not modify order state after it was cooked"
            );
            throw ex;
        }

        order.Status = OrderStatus.Delivery.ToString();
        await _context.SaveChangesAsync();

        Notify(order);
    }

    public async Task TakeOrder(Guid orderId, Guid cookId)
    {
        var cook = await InitCook(cookId);

        var order = await _context
            .Orders
            .Include(o => o.UserId)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                $"Order with id \"{orderId}\" was not found"
            );
            throw ex;
        }

        if (order.Restaurant != cook.Restaurant)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Cook can not take orders from other restaurant"
            );
            throw ex;
        }

        if (order.Status != OrderStatus.Created.ToString())
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                "Cook can not take orders that are not in created state"
            );
            throw ex;
        }

        order.Status = OrderStatus.Kitchen.ToString();
        cook.Orders.Add(order);

        await _context.SaveChangesAsync();

        Notify(order);
    }

    private static IEnumerable<OrderInfoDto> SortOrders(IEnumerable<OrderInfoDto> orders, GetOrdersCookListQuery query)
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

    private async Task<Cook> InitCook(Guid cookId)
    {
        var cook = await _context
            .Cooks
            .Include(cook => cook.Orders)
            .FirstOrDefaultAsync(c => c.Id == cookId);
        if (cook != null) return cook;

        var newCook = new Cook { Id = cookId };

        await _context.Cooks.AddAsync(newCook);
        await _context.SaveChangesAsync();
        cook = newCook;

        return cook;
    }

    private void Notify(Order order)
    {
        var message = new NotificationMessage
        {
            OrderId = order.Id,
            UserId = order.UserId,
            Status = NotificationStatus.New,
            Text = $"You order number {order.Number} is in state: {order.Status}"
        };

        _producerService.SendMessage(message);
    }
}