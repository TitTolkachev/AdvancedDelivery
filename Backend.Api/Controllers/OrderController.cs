using DeliveryBackend.DTO;
using DeliveryBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryBackend.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    [HttpGet]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("{id}")]
    [SwaggerOperation(Summary = "Get information about concrete order")]
    public async Task<OrderDto> GetOrderInfo(Guid id)
    {
        return await _orderService.GetOrderInfo(Guid.Parse(User.Identity.Name), id);
    }
    
    [HttpGet]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [SwaggerOperation(Summary = "Get a list of orders")]
    public async Task<List<OrderInfoDto>> GetOrders()
    {
        return await _orderService.GetOrders(Guid.Parse(User.Identity.Name));
    }
    
    [HttpPost]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [SwaggerOperation(Summary = "Creating the order from dishes in basket")]
    public async Task CreateOrder([FromBody] OrderCreateDto orderCreateDto)
    {
        await _orderService.CreateOrder(Guid.Parse(User.Identity.Name), orderCreateDto);
    }
    
    [HttpPost]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("{id}/status")]
    [SwaggerOperation(Summary = "Confirm order delivery")]
    public async Task ConfirmOrderDelivery(Guid id)
    {
        await _orderService.ConfirmOrderDelivery(Guid.Parse(User.Identity.Name), id);
    }
}