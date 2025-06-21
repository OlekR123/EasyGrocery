using Microsoft.EntityFrameworkCore;
using App_grocery_store.Models;

namespace App_grocery_store.Data
{
    public class GroceryDbContext : DbContext
    {
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public GroceryDbContext() : base(GetOptions()) { }

        private static DbContextOptions<GroceryDbContext> GetOptions()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            return new DbContextOptionsBuilder<GroceryDbContext>()
                .UseNpgsql(connectionString)
                .Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("grocery_store");

            modelBuilder.Entity<UserRole>().ToTable("vw_user_roles").HasNoKey();
            modelBuilder.Entity<User>().ToTable("vw_users").HasNoKey();
            modelBuilder.Entity<Category>().ToTable("vw_categories").HasNoKey();
            modelBuilder.Entity<Supplier>().ToTable("vw_suppliers").HasNoKey();
            modelBuilder.Entity<Product>().ToTable("vw_products").HasNoKey();
            modelBuilder.Entity<Order>().ToTable("vw_orders").HasNoKey();
            modelBuilder.Entity<OrderItem>().ToTable("vw_order_items").HasNoKey();
        }
    }
}