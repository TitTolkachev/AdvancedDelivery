using System.ComponentModel.DataAnnotations;

namespace DeliveryBackend.DTO;

public class Restaurant
{
    public Guid Id { get; set; }
    [Required] 
    [MinLength(1)] 
    public string Name { get; set; } = null!;
}