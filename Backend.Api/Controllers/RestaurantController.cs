using Backend.Common.Dto;
using DeliveryBackend.DTO.Queries;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryBackend.Controllers;

[ApiController]
[Route("api/restaurant")]
public class RestaurantController : ControllerBase
{
    
    [HttpGet]
    [SwaggerOperation(Summary = "Get a list of restaurants")]
    public async Task<RestaurantPagedList> GetRestaurantList([FromQuery] GetRestaurantListQuery restaurantListQuery)
    {
        // TODO(Не сделано)
        return null;
    }
}