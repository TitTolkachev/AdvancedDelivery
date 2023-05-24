﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryBackend.Controllers;

[Authorize]
[Authorize(Roles = "Courier")]
[Authorize(Policy = "ValidateToken")]
[Route("api/courier")]
public class CourierController : ControllerBase
{
    // private readonly IOrderService _orderService;
    // private readonly ICourierService _courierService;
    //
    // public CourierController(IOrderService orderService, ICourierService courierService)
    // {
    //     _orderService = orderService;
    //     _courierService = courierService;
    // }
    //
    // /// <summary>
    // /// Get list of courier's orders (current == true => taken, else => available)
    // /// </summary>
    // [HttpGet]
    // [Route("orders")]
    // public async Task<OrdersPageDto> GetCourierOrders([FromQuery] OrderQueryModel query)
    // {
    //     var courierInfo = GetCourierInfo(HttpContext.User);
    //     return await _orderService.GetOrders(query, UserRole.Courier, courierInfo);
    // }
    //
    // /// <summary>
    // /// Change order status when delivered it
    // /// </summary>
    // [HttpPut]
    // [Route("{orderId}")]
    // public async Task SetOrderStatusDelivered(Guid orderId)
    // {
    //     var courierInfo = GetCourierInfo(HttpContext.User);
    //     await _courierService.SetOrderDelivered(orderId, courierInfo);
    // }
    //
    // /// <summary>
    // /// Courier takes order
    // /// </summary>
    // [HttpPost]
    // [Route("{orderId}")]
    // public async Task TakeOrder(Guid orderId)
    // {
    //     var courierInfo = GetCourierInfo(HttpContext.User);
    //     await _courierService.TakeOrder(orderId, courierInfo);
    // }
    //
    // /// <summary>
    // /// Courier takes order
    // /// </summary>
    // [HttpPost]
    // [Route("cancel/{orderId}")]
    // public async Task CancelOrder(Guid orderId)
    // {
    //     var courierInfo = GetCourierInfo(HttpContext.User);
    //     await _courierService.CancelOrder(orderId, courierInfo);
    // }
}