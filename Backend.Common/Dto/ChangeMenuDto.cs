using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class ChangeMenuDto
{
    [Required] public Guid RestaurantId { get; set; }
    [Required] public string Name { get; set; } = null!;
}