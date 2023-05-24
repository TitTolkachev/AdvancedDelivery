using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminPanel.Common.Models;

public class User
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public Gender Gender { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public List<Role> Roles { get; set; } = new();

    public List<SelectListItem> RestaurantIds { get; set; } = new();

    public string? RestaurantId { get; set; }

    public string? Address { get; set; }
}