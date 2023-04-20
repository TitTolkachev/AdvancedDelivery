using System.ComponentModel.DataAnnotations;

namespace DeliveryBackend.DTO;

public class RestaurantPagedList
{
    [Required] 
    public List<Restaurant> Restaurants { get; set; }
    [Required]
    public PageInfoModel Pagination { get; set; }
}