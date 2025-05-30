﻿using System.ComponentModel.DataAnnotations;

namespace ProyectoWeb.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}


