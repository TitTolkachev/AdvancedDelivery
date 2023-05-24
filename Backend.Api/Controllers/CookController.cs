using System.Security.Claims;
using Backend.Common.Dto;
using Backend.Common.Dto.Queries;
using Backend.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryBackend.Controllers;

[Authorize]
[Authorize(Roles = "Cook")]
[Route("api/cook")]
public class CookController : ControllerBase
{
    private readonly ICookService _cookService;

    public CookController(ICookService cookService)
    {
        _cookService = cookService;
    }

    [HttpGet]
    [Route("orders/free")]
    [SwaggerOperation(Summary =
        "Get free orders. Sorting: CreateDateAsc, CreateDateDesc, CookedDateAsc, CookedDateDesc")]
    public async Task<OrderPagedListDto> GetFreeOrders([FromQuery] GetOrdersCookListQuery query)
    {
        return await _cookService.GetFreeOrders(query, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
    }

    [HttpGet]
    [Route("orders/history")]
    [SwaggerOperation(Summary = "Get cooked orders")]
    public async Task<OrderPagedListDto> GetOrdersFromHistory([FromQuery] GetOrdersListQuery query)
    {
        return await _cookService.GetCookedOrders(query, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
    }

    [HttpPatch]
    [Route("packed/{orderId}")]
    [SwaggerOperation(Summary = "Change order status to Packed")]
    public async Task SetOrderStatusPacked(Guid orderId)
    {
        await _cookService.SetOrderStatusPackaging(orderId, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
    }

    [HttpPost]
    [Route("take/{orderId}")]
    [SwaggerOperation(Summary = "Take order")]
    public async Task TakeOrder(Guid orderId)
    {
        await _cookService.TakeOrder(orderId, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
    }
}