using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Database;

public class RestaurantOrderingContextFactory : IDesignTimeDbContextFactory<RestaurantOrderingContext>
{
    public RestaurantOrderingContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RestaurantOrderingContext>();
        optionsBuilder.UseSqlite("YourConnectionStringHere");

        return new RestaurantOrderingContext(optionsBuilder.Options);
    }
}

public class RestaurantOrderingContext : DbContext
{
    public RestaurantOrderingContext(DbContextOptions<RestaurantOrderingContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<MenuCategory> MenuCategories { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<MenuItemTag> MenuItemTags { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<OrderItemIngredient> OrderItemIngredients { get; set; }
    public DbSet<CustomerInformation> CustomerInformations { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<MenuItemSale> MenuItemSales { get; set; }
    public DbSet<SalesRevenue> SalesRevenues { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure enums as strings
        modelBuilder.Entity<Order>()
            .Property(o => o.OrderStatus)
            .HasConversion(
                os => os.ToString(),
                os => (OrderStatus)Enum.Parse(typeof(OrderStatus), os)
            );

        modelBuilder.Entity<Order>()
            .Property(o => o.OrderType)
            .HasConversion(
                ot => ot.ToString(),
                ot => (OrderType)Enum.Parse(typeof(OrderType), ot)
            );

        modelBuilder.Entity<Payment>()
            .Property(p => p.PaymentMethod)
            .HasConversion(
                pm => pm.ToString(),
                pm => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), pm)
            );

        modelBuilder.Entity<Ingredient>()
            .Property(i => i.IngredientType)
            .HasConversion(
                it => it.ToString(),
                it => (IngredientType)Enum.Parse(typeof(IngredientType), it)
            );

        // Order and OrderItem relationship
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId);

        // One-to-One: Order and CustomerInformation
        modelBuilder.Entity<Order>()
            .HasOne(o => o.CustomerInformation)
            .WithOne(ci => ci.Order)
            .HasForeignKey<CustomerInformation>(ci => ci.OrderId);

        // One-To-Many Order and Payments relationship
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Payments)
            .WithOne(p => p.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderItem and MenuItem relationship
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.MenuItem)
            .WithMany()
            .HasForeignKey(oi => oi.MenuItemId);

        // MenuCategory and MenuItem relationship
        modelBuilder.Entity<MenuCategory>()
            .HasMany(mc => mc.MenuItems)
            .WithOne(mi => mi.MenuCategory)
            .HasForeignKey(mi => mi.MenuCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // MenuItem and MenuCategory relationship
        modelBuilder.Entity<MenuItem>()
            .HasOne(mi => mi.MenuCategory)
            .WithMany(mc => mc.MenuItems)
            .HasForeignKey(mi => mi.MenuCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Table and Order relationship
        modelBuilder.Entity<Table>()
            .HasMany(t => t.Orders)
            .WithOne(o => o.Table)
            .HasForeignKey(o => o.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        // Table and Reservation relationship
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Table)
            .WithMany(t => t.Reservations)
            .HasForeignKey(r => r.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        // MenuItem and Tag (Many-to-Many)
        modelBuilder.Entity<MenuItemTag>()
            .HasKey(mt => new { mt.MenuItemId, mt.TagId });

        modelBuilder.Entity<MenuItemTag>()
            .HasOne(mt => mt.MenuItem)
            .WithMany(mi => mi.MenuItemTags)
            .HasForeignKey(mt => mt.MenuItemId);

        modelBuilder.Entity<MenuItemTag>()
           .HasOne(mt => mt.Tag)
           .WithMany(t => t.MenuItemTags)
           .HasForeignKey(mt => mt.TagId);

        // OrderItem and Ingredient (Many-to-Many)
        modelBuilder.Entity<OrderItemIngredient>()
            .HasKey(oii => new { oii.OrderItemId, oii.IngredientId });

        modelBuilder.Entity<OrderItemIngredient>()
            .HasOne(oii => oii.OrderItem)
            .WithMany(oi => oi.OrderItemIngredients)
            .HasForeignKey(oii => oii.OrderItemId);

        modelBuilder.Entity<OrderItemIngredient>()
            .HasOne(oii => oii.Ingredient)
            .WithMany(i => i.OrderItemIngredients)
            .HasForeignKey(oii => oii.IngredientId);
    }
}