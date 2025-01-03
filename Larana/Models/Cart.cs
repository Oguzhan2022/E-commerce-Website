using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Larana.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
