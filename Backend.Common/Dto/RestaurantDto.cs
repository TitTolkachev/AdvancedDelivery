using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class RestaurantDto
{
    public Guid Id { get; set; }
    [Required] [MinLength(1)] public string Name { get; set; } = null!;
    [Required] public List<MenuDto> Menus { get; set; } = new();
}