using Microsoft.EntityFrameworkCore;
using InventoryAPI.Models;

namespace InventoryAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category constraints
            modelBuilder.Entity<Category>()
                .Property(c => c.CategoryName)
                .IsRequired()
                .HasMaxLength(100);

            // Category → User FK
            modelBuilder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product constraints
            modelBuilder.Entity<Product>()
                .Property(p => p.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Product>()
                .Property(p => p.SKU)
                .IsRequired()
                .HasMaxLength(50);

            // SKU unique per user (not globally unique anymore)
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.UserId, p.SKU })
                .IsUnique();

            modelBuilder.Entity<Product>()
                .Property(p => p.PurchasePrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.SellingPrice)
                .HasColumnType("decimal(18,2)");

            // Product → Category FK
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Product → User FK
            modelBuilder.Entity<Product>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order → User FK
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order → Product FK
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Product)
                .WithMany()
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .Property(o => o.UnitSellingPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.UnitPurchasePrice)
                .HasColumnType("decimal(18,2)");

            // Unique username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Seed only the admin user — no seeded categories/products
            // (each user will create their own data after login)
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "$2b$10$S5M9J1T4ERzGUwjPPurRKuCEJm5yoBze5thgw9/MqwZtNwLrAtUIi"
            });
        }
    }
}
