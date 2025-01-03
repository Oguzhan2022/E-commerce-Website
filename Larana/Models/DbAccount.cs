using System;
using System.Data.Entity;
using System.Linq;


namespace Larana.Models
{
    public class DbAccount : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}

