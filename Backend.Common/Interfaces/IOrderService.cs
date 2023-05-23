using Backend.Common.Dto;
using Backend.Common.Dto.Queries;

namespace Backend.Common.Interfaces;

public interface IOrderService
{
    public Task<OrderDto> GetOrderInfo(Guid userId, Guid orderId);
    public Task<OrderPagedListDto> GetOrders(Guid userId, GetOrdersListQuery getOrdersListQuery);
    public Task CreateOrder(Guid userId, OrderCreateDto orderCreateDto);
    public Task ConfirmOrderDelivery(Guid userId, Guid orderId);
    public Task RepeatOrder(Guid parse, OrderRepeatDto orderRepeatDto);     
}