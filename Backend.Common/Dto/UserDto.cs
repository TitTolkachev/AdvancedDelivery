using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class UserDto
{
    public Guid Id { get; set; }
    [Required] [MinLength(1)] public string FullName { get; set; } = null!;

    public DateTime? BirthDate { get; set; }
    [Required] public string Gender { get; set; } = null!;

    public string? Address { get; set; }
    [MinLength(1)] [EmailAddress] public string? Email { get; set; }
    [Phone] public string? PhoneNumber { get; set; }
}