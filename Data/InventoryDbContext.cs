using Microsoft.EntityFrameworkCore;
using ProductInventoryAPI.Models;

namespace ProductInventoryAPI.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsActive);
        });

        // Seed data
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, StockQuantity = 15, Category = "Electronics" },
            new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse", Price = 29.99m, StockQuantity = 3, Category = "Electronics" },
            new Product { Id = 3, Name = "Desk Chair", Description = "Ergonomic office chair", Price = 199.99m, StockQuantity = 8, Category = "Furniture" }
        );
    }
}