using Backend.Common.Dto;
using Backend.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryBackend.Controllers;

[ApiController]
[Route("api/basket")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;
    
    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }
    
    [HttpGet]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [SwaggerOperation(Summary = "Get user cart")]
    public async Task<List<DishBasketDto>> GetUserCart()
    {
        return await _basketService.GetUserCart(Guid.Parse(User.Identity.Name));
    }
    
    [HttpPost]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("dish/{dishId}")]
    [SwaggerOperation(Summary = "Add dish to cart")]
    public async Task AddDishToCart(Guid dishId)
    {
        await _basketService.AddDishToCart(dishId, Guid.Parse(User.Identity.Name));
    }
    
    [HttpDelete]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("dish/{dishId}")]
    [SwaggerOperation(Summary = "Decrease the number of dishes in the cart(if increase = true), or remove the dish completely(increase = false)")]
    public async Task DecreaseDishQuantityInCart(Guid dishId)
    {
        await _basketService.RemoveDishFromCart(dishId, Guid.Parse(User.Identity.Name));
    }
}