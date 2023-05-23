using System.ComponentModel.DataAnnotations;

namespace Auth.Common.Dto;

public class ProfileRequest
{
    [Required] [MinLength(2)] public string FullName { get; set; } = null!;

    [Required] [Phone] public string? Phone { get; set; }

    public string? Address { get; set; }
}