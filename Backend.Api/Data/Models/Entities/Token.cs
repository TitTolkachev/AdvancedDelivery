using System.ComponentModel.DataAnnotations;

namespace DeliveryBackend.Data.Models.Entities;

public class Token
{
    [Required]
    public string InvalidToken { get; set; }
    [Required]
    public DateTime ExpiredDate { get; set; }
}