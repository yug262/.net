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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Entity Constraints manually to complement DataAnnotations
            modelBuilder.Entity<Category>()
                .Property(c => c.CategoryName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Product>()
                .Property(p => p.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Product>()
                .Property(p => p.SKU)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .Property(p => p.PurchasePrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.SellingPrice)
                .HasColumnType("decimal(18,2)");

            // Configure Navigation Properties / Constraints
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Unique User index
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // ====== SEED DATA ======

            // 1 admin user
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "$2b$10$S5M9J1T4ERzGUwjPPurRKuCEJm5yoBze5thgw9/MqwZtNwLrAtUIi"
            });

            // 3 categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, CategoryName = "Electronics", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 2, CategoryName = "Clothing", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 3, CategoryName = "Books", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            // 5 sample products
            modelBuilder.Entity<Product>().HasData(
                new Product 
                { 
                    Id = 1, ProductName = "Laptop XPS", SKU = "ELEC-LXP-01", CategoryId = 1, 
                    PurchasePrice = 800m, SellingPrice = 999.99m, Quantity = 10, 
                    Description = "High performance laptop", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) 
                },
                new Product 
                { 
                    Id = 2, ProductName = "Wireless Mouse", SKU = "ELEC-MSE-02", CategoryId = 1, 
                    PurchasePrice = 15m, SellingPrice = 29.99m, Quantity = 3, // Low stock 
                    Description = "Ergonomic wireless mouse", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) 
                },
                new Product 
                { 
                    Id = 3, ProductName = "T-Shirt Classic", SKU = "CLO-TSH-01", CategoryId = 2, 
                    PurchasePrice = 8m, SellingPrice = 19.99m, Quantity = 50, 
                    Description = "Cotton classic t-shirt", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) 
                },
                new Product 
                { 
                    Id = 4, ProductName = "Design Patterns", SKU = "BOK-DP-01", CategoryId = 3, 
                    PurchasePrice = 30m, SellingPrice = 45.00m, Quantity = 0, // Out of stock
                    Description = "Software engineering book", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) 
                },
                new Product 
                { 
                    Id = 5, ProductName = "Clean Code", SKU = "BOK-CC-02", CategoryId = 3, 
                    PurchasePrice = 25m, SellingPrice = 40.00m, Quantity = 12, 
                    Description = "A Handbook of Agile Software Craftsmanship", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) 
                }
            );
        }
    }
}
