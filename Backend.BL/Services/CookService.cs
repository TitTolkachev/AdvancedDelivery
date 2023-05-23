using Backend.Common.Dto;
using Backend.Common.Dto.Queries;
using Backend.Common.Interfaces;

namespace Backend.BL.Services;

public class CookService : ICookService
{
    public Task<OrderPagedListDto> GetFreeOrders(GetOrdersCookListQuery query, Guid cookId)
    {
        throw new NotImplementedException();
    }

    public Task<OrderPagedListDto> GetCookedOrders(GetOrdersListQuery query, Guid cookId)
    {
        throw new NotImplementedException();
    }

    public Task SetOrderStatusCooked(Guid orderId, Guid cookId)
    {
        throw new NotImplementedException();
    }

    public Task SetOrderStatusPacked(Guid orderId, Guid cookId)
    {
        throw new NotImplementedException();
    }

    public Task TakeOrder(Guid orderId, Guid cookId)
    {
        throw new NotImplementedException();
    }
}