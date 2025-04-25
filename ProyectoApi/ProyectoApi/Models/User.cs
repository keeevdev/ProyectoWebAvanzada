using System;

namespace ProyectoApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required byte[] PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
