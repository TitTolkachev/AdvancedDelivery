﻿using Backend.Common.Dto;
using Backend.Common.Dto.Queries;

namespace Backend.Common.Interfaces;

public interface ICookService
{
    Task<OrderPagedListDto> GetFreeOrders(GetOrdersCookListQuery query, Guid cookId);
    Task<OrderPagedListDto> GetCookedOrders(GetOrdersListQuery query, Guid cookId);
    Task SetOrderStatusPackaging(Guid orderId, Guid cookId);
    Task TakeOrder(Guid orderId, Guid cookId);
}