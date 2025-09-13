using Feed_Bridge.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;

namespace Feed_Bridge.Models.Data
{
    public class AppDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Support> Supports { get; set; }
        public DbSet<StaticPage> StaticPages { get; set; }
        public DbSet<Partener> Parteners { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        //  public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<ProductCart> ProductCarts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // int مش string ك database يتشاف في ال Order Enum عملت كدا عشان ال 
            builder.Entity<Order>()
                .Property(x => x.Status)
                .HasConversion<string>();

            builder.Entity<ProductCart>().HasOne(x => x.Cart)
                .WithMany(x => x.ProductCarts).HasForeignKey(x => x.CartId);

            builder.Entity<ProductCart>().HasOne(x => x.Product)
               .WithMany(x => x.ProductCarts).HasForeignKey(x => x.ProductId);

            // Composite PK => Id property ال OrderProduct class لاني معملتش في 
            builder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            builder.Entity<OrderProduct>().HasOne(x => x.Order)
                .WithMany(x => x.OrderProducts).HasForeignKey(x => x.OrderId);

            builder.Entity<OrderProduct>().HasOne(x => x.Product)
                .WithMany(x => x.OrderProducts).HasForeignKey(x => x.ProductId);

            builder.Entity<Donation>()
                .HasOne(d => d.Delivery)
                .WithMany(u => u.Deliveries)
                .HasForeignKey(d => d.DeliveryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Donation>()
                .HasOne(d => d.User)
                .WithMany(u => u.Donnations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.SetNull);
 
            // Seed Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "Delivery", NormalizedName = "DELIVERY" },
                new IdentityRole { Id = "3", Name = "User", NormalizedName = "USER" }
            );



        }

    }
}
