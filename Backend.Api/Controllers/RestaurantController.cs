using Backend.Common.Dto;
using Backend.Common.Dto.Queries;
using Backend.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryBackend.Controllers;

[ApiController]
[Route("api/restaurant")]
public class RestaurantController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet]
    [Route("search")]
    [SwaggerOperation(Summary = "Get a list of restaurants (with search option)")]
    public async Task<RestaurantPagedListDto> GetRestaurants([FromQuery] GetRestaurantListQuery restaurantListQuery)
    {
        return await _restaurantService.GetRestaurantList(restaurantListQuery);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get restaurant by Id")]
    public async Task<RestaurantDto> GetRestaurant(Guid restaurantId)
    {
        return await _restaurantService.GetRestaurant(restaurantId);
    }
}