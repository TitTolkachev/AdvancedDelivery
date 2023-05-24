using System.Security.Claims;
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
    [SwaggerOperation(Summary = "Get user cart")]
    public async Task<List<DishBasketDto>> GetUserCart()
    {
        return await _basketService.GetUserCart(GetInfo(HttpContext.User), Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
    }

    [HttpPost]
    [Authorize]
    [Route("dish/{dishId}")]
    [SwaggerOperation(Summary = "Add dish to cart")]
    public async Task AddDishToCart(Guid dishId)
    {
        await _basketService.AddDishToCart(GetInfo(HttpContext.User), dishId, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
    }

    [HttpDelete]
    [Authorize]
    [Route("dish/{dishId}")]
    [SwaggerOperation(Summary = "Decrease the number of dishes in the cart")]
    public async Task DecreaseDishQuantityInCart(Guid dishId)
    {
        await _basketService.RemoveDishFromCart(GetInfo(HttpContext.User), dishId, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
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