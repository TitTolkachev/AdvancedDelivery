﻿using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Dto;

public class DishDto
{
    public Guid Id { get; set; }
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    [Required]
    public double Price { get; set; }
    public string? Image { get; set; }
    [Required]
    public bool Vegetarian { get; set; }
    public double? Rating { get; set; }
    [Required]
    public string Category { get; set; } = null!;
}