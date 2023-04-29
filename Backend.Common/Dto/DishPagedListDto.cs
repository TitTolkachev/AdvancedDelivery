using System.ComponentModel.DataAnnotations;
using DeliveryBackend.DTO;

namespace Backend.Common.Dto;

public class DishPagedListDto
{
    [Required] 
    public List<DishDto> Dishes { get; set; }
    [Required]
    public PageInfoModel Pagination { get; set; }
}