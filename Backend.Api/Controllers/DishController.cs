using DeliveryBackend.DTO;
using DeliveryBackend.DTO.Queries;
using DeliveryBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryBackend.Controllers;

[ApiController]
[Route("api/dish")]
public class DishController : ControllerBase
{
    private readonly IDishService _dishService;

    public DishController(IDishService dishService)
    {
        _dishService = dishService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get a list of dishes (menu)")]
    public async Task<DishPagedListDto> GetDishList([FromQuery] GetDishListQuery dishListQuery)
    {
        return await _dishService.GetDishList(dishListQuery);
    }

    [HttpGet]
    [Route("{id}")]
    [SwaggerOperation(Summary = "Get information about concrete dish")]
    public async Task<DishDto> GetDish(Guid id)
    {
        return await _dishService.GetDish(id);
    }

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("{id}/rating/check")]
    [SwaggerOperation(Summary = "Checks if user is able to set rating of the dish")]
    public async Task<bool> CheckDishRating(Guid id)
    {
        return await _dishService.CheckDishRating(id, Guid.Parse(User.Identity.Name));
    }

    [HttpPost]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("{id}/rating")]
    [SwaggerOperation(Summary = "Set a rating for a dish")]
    public async Task SetDishRating(Guid id, [FromQuery] int rating)
    {
        await _dishService.SetDishRating(id, rating, Guid.Parse(User.Identity.Name));
    }
    
    
    // --------------------
    // --------------------
    [HttpPost]
    [SwaggerOperation(Summary = "~*!EXTRA!*~ (ADD DISHES)")]
    public async Task AddDishes([FromBody] List<DishDto> dishes)
    {
        await _dishService.AddDishes(dishes);
    }
}