using Backend.Common.Dto;
using Backend.Common.Dto.Queries;

namespace Backend.Common.Interfaces;

public interface IManagerService
{
    Task<OrderPagedListDto> GetOrders(GetOrdersManagerListQuery query, Guid managerId);
    Task CreateMenu(CreateMenuDto createMenuDto, Guid managerId);
    Task ChangeMenu(Guid menuId, ChangeMenuDto changeMenuDto, Guid managerId);
    Task DeleteMenu(Guid menuId, Guid managerId);
    Task AddDishToMenu(Guid menuId, Guid dishId, Guid managerId);
    Task RemoveDishFromMenu(Guid menuId, Guid dishId, Guid managerId);
}