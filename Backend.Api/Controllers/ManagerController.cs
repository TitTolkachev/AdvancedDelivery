using Backend.Common.Dto;
using Backend.Common.Dto.Queries;
using Backend.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryBackend.Controllers;

[Authorize]
[Authorize(Roles = "Manager")]
[Authorize(Policy = "ValidateToken")]
[Route("api/manager")]
public class ManagerController : ControllerBase
{
    private readonly IManagerService _managerService;

    public ManagerController(IManagerService managerService)
    {
        _managerService = managerService;
    }

    [HttpGet]
    [Route("orders")]
    [SwaggerOperation(Summary = "Get restaurant orders")]
    public async Task<OrderPagedListDto> GetOrders([FromQuery] GetOrdersManagerListQuery query)
    {
        return await _managerService.GetOrders(query, Guid.Parse(User.Identity.Name));
    }

    [HttpPost]
    [Route("menu/new")]
    [SwaggerOperation(Summary = "Create new menu")]
    public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto createMenuDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _managerService.CreateMenu(createMenuDto, Guid.Parse(User.Identity.Name));
        return Ok();
    }

    [HttpPut]
    [Route("menu/{menuId}")]
    [SwaggerOperation(Summary = "Change menu")]
    public async Task ChangeMenu(Guid menuId, [FromBody] ChangeMenuDto changeMenuDto)
    {
        await _managerService.ChangeMenu(menuId, changeMenuDto, Guid.Parse(User.Identity.Name));
    }

    [HttpDelete]
    [Route("menu/{menuId}")]
    [SwaggerOperation(Summary = "Delete menu")]
    public async Task DeleteMenu(Guid menuId)
    {
        await _managerService.DeleteMenu(menuId, Guid.Parse(User.Identity.Name));
    }

    [HttpPost]
    [Route("menu/{menuId}/{dishId}")]
    [SwaggerOperation(Summary = "Add dish to menu")]
    public async Task AddDishToMenu(Guid menuId, Guid dishId)
    {
        await _managerService.AddDishToMenu(menuId, dishId, Guid.Parse(User.Identity.Name));
    }

    [HttpDelete]
    [Route("menu/{menuId}/{dishId}")]
    [SwaggerOperation(Summary = "Remove dish from menu")]
    public async Task RemoveDishMenu(Guid menuId, Guid dishId)
    {
        await _managerService.RemoveDishFromMenu(menuId, dishId, Guid.Parse(User.Identity.Name));
    }
}