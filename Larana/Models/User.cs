using System;
using System.ComponentModel.DataAnnotations;

namespace Larana.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Roles { get; set; }

        public User()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
