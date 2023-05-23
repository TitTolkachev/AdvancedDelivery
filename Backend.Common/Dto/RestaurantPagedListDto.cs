using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class RestaurantPagedListDto
{
    [Required] public List<RestaurantDto>? Restaurants { get; set; }
    [Required] public PageInfoModel? Pagination { get; set; }
}