using AutoMapper;
using Backend.BL.Enums;
using Backend.Common.Dto;
using Backend.Common.Interfaces;
using Backend.DAL;
using Backend.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Backend.BL.Services;

public class CourierService : ICourierService
{
    private readonly ApplicationDbContext _context;
    private readonly IProducerService _producerService;
    private readonly IMapper _mapper;

    public CourierService(ApplicationDbContext context, IProducerService producerService, IMapper mapper)
    {
        _context = context;
        _producerService = producerService;
        _mapper = mapper;
    }

    public async Task<List<OrderInfoDto>> GetOrders(Guid courierId)
    {
        await InitCourier(courierId);

        var orders = await _context.Orders.Where(x => x.Status == OrderStatus.Packaging.ToString()).ToListAsync();

        return _mapper.Map<List<OrderInfoDto>>(orders);
    }

    public async Task TakeOrder(Guid orderId, Guid courierId)
    {
        var courier = await InitCourier(courierId);

        var order = await _context
            .Orders
            .Include(order => order.User)
            .FirstOrDefaultAsync(order => order.Id == orderId);
        if (order == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Order was not found"
            );
            throw ex;
        }

        if (order.Status != OrderStatus.Packaging.ToString())
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                "Courier can not take orders that are not in Packaging state"
            );
            throw ex;
        }

        order.Status = OrderStatus.Delivery.ToString();
        courier.Orders.Add(order);

        await _context.SaveChangesAsync();

        Notify(order);
    }

    public async Task SetOrderDelivered(Guid orderId, Guid courierId)
    {
        var order = await _context
            .Orders
            .Include(order => order.Courier)
            .Include(order => order.User)
            .FirstOrDefaultAsync(or => or.Id == orderId);
        if (order == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Order was not found"
            );
            throw ex;
        }

        if (order.CourierId != courierId)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                $"Courier with id \"{courierId}\" does not deliver order with id \"{orderId}\""
            );
            throw ex;
        }

        if (order.Status != OrderStatus.Delivery.ToString())
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                "Courier can not set orders Delivered state if they are not in Delivery state"
            );
            throw ex;
        }

        order.Status = OrderStatus.Delivered.ToString();
        await _context.SaveChangesAsync();

        Notify(order);
    }

    public async Task CancelOrder(Guid orderId, Guid courierId)
    {
        var order = await _context
            .Orders
            .Include(order => order.Courier)
            .Include(order => order.User)
            .FirstOrDefaultAsync(or => or.Id == orderId);
        if (order == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Order was not found"
            );
            throw ex;
        }

        if (order.CourierId != courierId)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                $"Courier with id \"{courierId}\" does not deliver order with id \"{orderId}\""
            );
            throw ex;
        }

        if (order.Status != OrderStatus.Delivery.ToString())
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                "Courier can not cancel orders that are not in Delivery state"
            );
            throw ex;
        }

        order.Status = OrderStatus.Canceled.ToString();
        await _context.SaveChangesAsync();

        Notify(order);
    }

    private void Notify(Order order)
    {
        var orderStatusMessage = new NotificationMessage
        {
            OrderId = order.Id,
            UserId = order.UserId,
            Status = NotificationStatus.New,
            Text = $"You order number {order.Number} is in state: {order.Status}"
        };

        _producerService.SendMessage(orderStatusMessage);
    }

    private async Task<Courier> InitCourier(Guid courierId)
    {
        var courier = await _context
            .Couriers
            .Include(cour => cour.Orders)
            .FirstOrDefaultAsync(c => c.Id == courierId);
        if (courier != null) return courier;

        var newCourier = new Courier
        {
            Id = courierId
        };

        await _context.Couriers.AddAsync(newCourier);
        await _context.SaveChangesAsync();
        courier = newCourier;

        return courier;
    }
}