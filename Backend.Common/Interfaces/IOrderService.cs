using Backend.Common.Dto;

namespace Backend.Common.Interfaces;

public interface IOrderService
{
    public Task<OrderDto> GetOrderInfo(Guid userId, Guid orderId);
    public Task<List<OrderInfoDto>> GetOrders(Guid userId);
    public Task CreateOrder(Guid userId, OrderCreateDto orderCreateDto);
    public Task ConfirmOrderDelivery(Guid userId, Guid orderId);
}