using System.ComponentModel.DataAnnotations;

namespace DeliveryBackend.DTO;

public class DishPagedListDto
{
    [Required] 
    public List<DishDto> Dishes { get; set; }
    [Required]
    public PageInfoModel Pagination { get; set; }
}