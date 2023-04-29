using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class RestaurantPagedList
{
    [Required] 
    public List<Restaurant> Restaurants { get; set; }
    [Required]
    public PageInfoModel Pagination { get; set; }
}