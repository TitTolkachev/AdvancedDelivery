﻿using System.ComponentModel.DataAnnotations;

namespace Backend.DAL.Entities;

public class Token
{
    [Required]
    public string InvalidToken { get; set; } = null!;

    [Required]
    public DateTime ExpiredDate { get; set; }
}