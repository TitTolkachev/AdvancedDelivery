using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class Restaurant
{
    public Guid Id { get; set; }
    [Required] 
    [MinLength(1)] 
    public string Name { get; set; } = null!;
}