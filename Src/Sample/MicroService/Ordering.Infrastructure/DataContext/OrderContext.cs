using Microsoft.EntityFrameworkCore;
using Ordering.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Infrastructure
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        
        public DbSet<Order> Orders { get; set; }
 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<Order>().HasMany(x => x.Items);


            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=.,14330;Initial Catalog=OrderDb;User ID=sa;Password=!fzf123456;MultipleActiveResultSets=true");
        }
    }
}
