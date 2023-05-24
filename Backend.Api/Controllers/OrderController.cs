using System.Security.Claims;
using Backend.Common.Dto;
using Backend.Common.Dto.Queries;
using Backend.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryBackend.Controllers;

[ApiController]
[Authorize]
[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [Route("{id}")]
    [SwaggerOperation(Summary = "Get information about concrete order")]
    public async Task<OrderDto> GetOrderInfo(Guid id)
    {
        return await _orderService.GetOrderInfo(GetInfo(HttpContext.User), Guid.Parse(User.Identity.Name), id);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get a list of orders")]
    public async Task<OrderPagedListDto> GetOrders([FromQuery] GetOrdersListQuery query)
    {
        return await _orderService.GetOrders(GetInfo(HttpContext.User), Guid.Parse(User.Identity.Name), query);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Creating the order from dishes in basket")]
    public async Task CreateOrder([FromBody] OrderCreateDto orderCreateDto)
    {
        await _orderService.CreateOrder(GetInfo(HttpContext.User), Guid.Parse(User.Identity.Name), orderCreateDto);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Creating the order from dishes in basket")]
    public async Task RepeatOrder([FromBody] OrderRepeatDto orderRepeatDto)
    {
        await _orderService.RepeatOrder(GetInfo(HttpContext.User), Guid.Parse(User.Identity.Name), orderRepeatDto);
    }

    [HttpPost]
    [Route("{id}/status")]
    [SwaggerOperation(Summary = "Confirm order delivery")]
    public async Task ConfirmOrderDelivery(Guid id)
    {
        await _orderService.ConfirmOrderDelivery(GetInfo(HttpContext.User), Guid.Parse(User.Identity.Name), id);
    }

    [HttpPost]
    [Route("cancel/{id}")]
    [SwaggerOperation(Summary = "Cancel order")]
    public async Task CancelOrder(Guid id)
    {
        // TODO(Не сделано)
    }

    private UserInfoDto GetInfo(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userAddress = user.FindFirst("address")?.Value;
        var userInfo = new UserInfoDto()
        {
            id = Guid.Parse(userId!),
            address = userAddress
        };

        return userInfo;
    }
}