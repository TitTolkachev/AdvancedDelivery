﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DAL.Entities;

public class Rating
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid DishId { get; set; }
    [Required]
    [ForeignKey("DishId")]
    public Dish Dish { get; set; } = null!;

    [Required]
    public Guid UserId { get; set; }
    [Required]
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [Required]
    public int RatingScore { get; set; }
}