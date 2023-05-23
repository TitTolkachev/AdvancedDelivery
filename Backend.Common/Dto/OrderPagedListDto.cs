using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class OrderPagedListDto
{
    [Required] public List<OrderInfoDto> Orders { get; set; } = new();

    [Required] public PageInfoModel Pagination { get; set; } = null!;
}