using System.ComponentModel.DataAnnotations;

namespace Auth.Common.Dto;

public class AuthRequest
{
    [Required] 
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}