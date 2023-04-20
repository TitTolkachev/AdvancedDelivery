using System.ComponentModel.DataAnnotations;

namespace Auth.Common.Dto;

public class ProfileResponse
{
    [Required]
    [MinLength(2)]
    public string? FullName { get; set; } = null!;
    
    [Required]
    public DateTime? BirthDate { get; set; }
    
    [Required]
    public string? Gender { get; set; }
    
    [Required]
    [Phone]
    public string? Phone { get; set; }
    
    [Required] 
    public string? Address { get; set; }
    
    [Required] 
    [MinLength(6)]
    [EmailAddress]
    public string Email { get; set; } = null!;
}