using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class UserRegisterModel
{
    [Required]
    [MinLength(1)]
    public string FullName { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;

    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    [Required]
    public string Gender { get; set; } = null!;

    [Phone]
    public string? PhoneNumber { get; set; }
}