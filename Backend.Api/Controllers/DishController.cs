﻿using Backend.Common.Dto;
using Backend.Common.Dto.Queries;
using Backend.Common.Interfaces;
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
    [SwaggerOperation(Summary =
        "Get a list of dishes from a restaurant. Sorting: NameAsc, NameDesc, PriceAsc, PriceDesc, RatingAsc, RatingDesc. " +
        "Categories: Wok, Pizza, Soup, Dessert, Drink")]
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
    [SwaggerOperation(Summary = "Check if user is able to set rating for the dish")]
    public async Task<bool> CheckDishRating(Guid id)
    {
        return await _dishService.CheckDishRating(id, Guid.Parse(User.Identity.Name));
    }

    [HttpPost]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("{id}/rating")]
    [SwaggerOperation(Summary = "Rate dish")]
    public async Task SetDishRating(Guid id, [FromQuery] int rating)
    {
        await _dishService.SetDishRating(id, rating, Guid.Parse(User.Identity.Name));
    }


    // TODO(Переделать)
    // --------------------
    // --------------------
    [HttpPost]
    [SwaggerOperation(Summary = "~*!EXTRA!*~ (ADD DISHES)")]
    public async Task AddDishes([FromBody] List<DishDto> dishes)
    {
        await _dishService.AddDishes(dishes);
    }
}