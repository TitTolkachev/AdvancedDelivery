using Backend.Common.Dto;
using Backend.Common.Dto.Queries;

namespace Backend.Common.Interfaces;

public interface IOrderService
{
    public Task<OrderDto> GetOrderInfo(UserInfoDto userInfoDto, Guid userId, Guid orderId);

    public Task<OrderPagedListDto> GetOrders(UserInfoDto userInfoDto, Guid userId,
        GetOrdersListQuery getOrdersListQuery);

    public Task CreateOrder(UserInfoDto userInfoDto, Guid userId, OrderCreateDto orderCreateDto);
    public Task ConfirmOrderDelivery(UserInfoDto userInfoDto, Guid userId, Guid orderId);
    public Task RepeatOrder(UserInfoDto userInfoDto, Guid parse, OrderRepeatDto orderRepeatDto);
}