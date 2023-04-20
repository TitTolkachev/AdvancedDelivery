using System.ComponentModel.DataAnnotations;

namespace Auth.Common.Dto;

public class ProfileResponse
{
    [Required]
    [MinLength(2)]
    [Display(Name = "Имя")]
    public string? FullName { get; set; } = null!;
    
    [Required]
    [Display(Name = "Дата рождения")]
    public DateTime? BirthDate { get; set; }
    
    [Required]
    [Display(Name = "Пол")]
    public string? Gender { get; set; }
    
    [Required]
    [Phone]
    [Display(Name = "Телефон")]
    public string? Phone { get; set; }
    
    [Required] 
    [Display(Name = "Адрес")] 
    public string? Address { get; set; }
    
    [Required] 
    [MinLength(6)]
    [EmailAddress]
    [Display(Name = "Email")] 
    public string Email { get; set; } = null!;
}