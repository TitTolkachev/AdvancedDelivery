using System.Security.Claims;
using Backend.Common.Dto;
using Backend.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryBackend.Controllers;

[Authorize]
[Authorize(Roles = "Courier")]
[Route("api/courier")]
public class CourierController : ControllerBase
{
    private readonly ICourierService _courierService;

    public CourierController(ICourierService courierService)
    {
        _courierService = courierService;
    }

    [HttpGet]
    [Route("orders")]
    [SwaggerOperation(Summary = "Get orders")]
    public async Task<List<OrderInfoDto>> GetOrders()
    {
        return await _courierService.GetOrders(Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
    }

    [HttpPost]
    [Route("{orderId}")]
    [SwaggerOperation(Summary = "Take order")]
    public async Task TakeOrder(Guid orderId)
    {
        await _courierService.TakeOrder(orderId, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
    }

    [HttpPatch]
    [Route("{orderId}")]
    [SwaggerOperation(Summary = "Change order status to delivered")]
    public async Task SetOrderDelivered(Guid orderId)
    {
        await _courierService.SetOrderDelivered(orderId, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
    }

    [HttpPost]
    [Route("cancel/{orderId}")]
    [SwaggerOperation(Summary = "Cancel order")]
    public async Task CancelOrder(Guid orderId)
    {
        await _courierService.CancelOrder(orderId, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
    }
}