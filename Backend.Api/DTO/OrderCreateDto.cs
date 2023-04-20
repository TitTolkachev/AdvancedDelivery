using System.ComponentModel.DataAnnotations;

namespace DeliveryBackend.DTO;

public class OrderCreateDto
{
    [Required]
    public DateTime DeliveryTime { get; set; }
    [Required]
    [MinLength(1)]
    public string Address { get; set; }
}