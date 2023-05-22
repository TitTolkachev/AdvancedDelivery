using Backend.Common.Dto;
using Backend.Common.Interfaces;
using DeliveryBackend.DTO.Queries;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryBackend.Controllers;

[ApiController]
[Route("api/restaurant")]
public class RestaurantController : ControllerBase
{
    private readonly IProducerService _producerService;

    public RestaurantController(IProducerService producerService)
    {
        _producerService = producerService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get a list of restaurants")]
    public async Task<RestaurantPagedList> GetRestaurantList([FromQuery] GetRestaurantListQuery restaurantListQuery)
    {
        // TODO(Не сделано)
        return null;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "TEST FOR RABBIT_MQ")]
    public async Task<OkObjectResult> Test()
    {
        // publish message  
        _producerService.SendMessage("Hello everyone!");

        return Ok(null);
    }
}