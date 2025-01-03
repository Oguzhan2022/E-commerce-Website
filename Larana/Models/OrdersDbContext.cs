using System;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity;
using Larana.Models;

namespace Larana.Data
{
    public class OrdersDbContext : DbContext
    {
        public OrdersDbContext()
            : base("OrdersConnection")
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithRequired(od => od.Order)
                .HasForeignKey(od => od.OrderId);
        }
    }
}
