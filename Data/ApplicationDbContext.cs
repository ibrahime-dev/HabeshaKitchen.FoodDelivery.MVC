using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using FoodDelivery.Models;

namespace FoodDelivery.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false) { }

        public static ApplicationDbContext Create() => new ApplicationDbContext();


        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Decimal precision
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MenuItem>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);


            // Restaurant - Owner
            modelBuilder.Entity<Restaurant>()
                .HasRequired(r => r.Owner)
                .WithMany()
                .HasForeignKey(r => r.OwnerId)
                .WillCascadeOnDelete(false);

            // Order - Restaurant (no cascade)
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Restaurant)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.RestaurantId)
                .WillCascadeOnDelete(false);

            // MenuItem - Restaurant cascade delete
            modelBuilder.Entity<MenuItem>()
                .HasRequired(m => m.Restaurant)
                .WithMany(r => r.MenuItems)
                .HasForeignKey(m => m.RestaurantId)
                .WillCascadeOnDelete(true);

            // Order - Customer (no cascade to avoid multiple cascade paths)
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Customer)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.CustomerId)
                .WillCascadeOnDelete(false);


            // OrderItem - MenuItem (no cascade)
            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.MenuItem)
                .WithMany(m => m.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId)
                .WillCascadeOnDelete(false);

            // Cart - User (one-to-many)
            modelBuilder.Entity<Cart>()
                .HasRequired(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .WillCascadeOnDelete(false);

            // CartItem - MenuItem (no cascade)
            modelBuilder.Entity<CartItem>()
                .HasRequired(ci => ci.MenuItem)
                .WithMany(m => m.CartItems)
                .HasForeignKey(ci => ci.MenuItemId)
                .WillCascadeOnDelete(false);

            // Payment - Order (one-to-one, each order has one payment)
            modelBuilder.Entity<Payment>()
                .HasRequired(p => p.Order)
                .WithMany()
                .HasForeignKey(p => p.OrderId)
                .WillCascadeOnDelete(true);


            // CartItem - Cart cascade delete
            modelBuilder.Entity<CartItem>()
                .HasRequired(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .WillCascadeOnDelete(true);

            // OrderItem - Order cascade delete
            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .WillCascadeOnDelete(true);
        }
    }
}
