using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class OrderInfoDto
{
    public Guid Id { get; set; }
    [Required]
    public DateTime DeliveryTime { get; set; }
    [Required]
    public DateTime OrderTime { get; set; }
    [Required]
    public string Status { get; set; } = null!;

    [Required]
    public decimal Price { get; set; }  
}