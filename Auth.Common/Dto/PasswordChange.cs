using System.ComponentModel.DataAnnotations;

namespace Auth.Common.Dto;

public class PasswordChange
{
    [Required]
    [MinLength(2)]
    public string CurrentPassword { get; set; } = null!;

    [Required]
    [MinLength(2)]
    public string NewPassword { get; set; } = null!;
}