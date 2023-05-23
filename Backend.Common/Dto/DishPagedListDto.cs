using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class DishPagedListDto
{
    [Required] 
    public List<DishDto> Dishes { get; set; } = null!;

    [Required]
    public PageInfoModel Pagination { get; set; } = null!;
}