using System.ComponentModel.DataAnnotations;

namespace Auth.Common.Dto;

public class AuthRequest
{
    [Required] 
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email")] 
    public string Email { get; set; } = null!;
    
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = null!;
}