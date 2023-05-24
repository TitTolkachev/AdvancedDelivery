using Backend.Common.Dto;

namespace Backend.Common.Interfaces;

public interface ICourierService
{
    Task<List<OrderInfoDto>> GetOrders(Guid courierId);
    Task TakeOrder(Guid orderId, Guid courierId);
    Task SetOrderDelivered(Guid orderId, Guid courierId);
    Task CancelOrder(Guid orderId, Guid courierId);
}