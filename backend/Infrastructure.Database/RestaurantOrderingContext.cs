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
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<MenuItemIngredientRel> MenuItemIngredientRels { get; set; }
    public DbSet<OrderItemIngredient> OrderItemIngredients { get; set; }
    public DbSet<CustomerInformation> CustomerInformations { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<MenuItemSale> MenuItemSales { get; set; }
    public DbSet<SalesRevenue> SalesRevenues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
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
        modelBuilder.Entity<Table>()
            .Property(t => t.TableStatus)
            .HasConversion(
                ts => ts.ToString(),
                ts => (TableStatus)Enum.Parse(typeof(TableStatus), ts)
            );


        // ✅ Order and OrderItem relationship (One-to-Many)
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId);

        // ✅ One-to-One: Order and CustomerInformation
        modelBuilder.Entity<Order>()
            .HasOne(o => o.CustomerInformation)
            .WithOne(ci => ci.Order)
            .HasForeignKey<CustomerInformation>(ci => ci.OrderId);

        // ✅ One-To-Many Order and Payments relationship
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Payments)
            .WithOne(p => p.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // ✅ OrderItem and MenuItem relationship (Many-to-One)
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.MenuItem)
            .WithMany()
            .HasForeignKey(oi => oi.MenuItemId);

        // ✅ MenuCategory and MenuItem relationship
        modelBuilder.Entity<MenuCategory>()
            .HasMany(mc => mc.MenuItems)
            .WithOne(mi => mi.MenuCategory)
            .HasForeignKey(mi => mi.MenuCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // ✅ Table and Order relationship
        modelBuilder.Entity<Table>()
            .HasMany(t => t.Orders)
            .WithOne(o => o.Table)
            .HasForeignKey(o => o.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        // ✅ Table and Reservation relationship
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Table)
            .WithMany(t => t.Reservations)
            .HasForeignKey(r => r.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        // ✅ Many-to-Many: MenuItem and Ingredient
        modelBuilder.Entity<MenuItemIngredientRel>()
            .HasKey(mi => new { mi.MenuItemId, mi.IngredientId });

        modelBuilder.Entity<MenuItemIngredientRel>()
            .HasOne(mi => mi.MenuItem)
            .WithMany(m => m.MenuItemIngredientRels)
            .HasForeignKey(mi => mi.MenuItemId);

        modelBuilder.Entity<MenuItemIngredientRel>()
            .HasOne(mi => mi.Ingredient)
            .WithMany(i => i.MenuItemIngredientRels)
            .HasForeignKey(mi => mi.IngredientId);

        // ✅ Many-to-Many: OrderItem and Ingredient (Custom order modifications)
        modelBuilder.Entity<OrderItemIngredient>()
            .HasKey(oii => new { oii.OrderItemId, oii.IngredientId });

        modelBuilder.Entity<OrderItemIngredient>()
            .HasOne(oii => oii.OrderItem)
            .WithMany(oi => oi.Ingredients)
            .HasForeignKey(oii => oii.OrderItemId);

        modelBuilder.Entity<OrderItemIngredient>()
            .HasOne(oii => oii.Ingredient)
            .WithMany()
            .HasForeignKey(oii => oii.IngredientId);

        // ✅ Many-to-Many: Ingredient and Tag
        modelBuilder.Entity<IngredientTagRel>()
            .HasKey(it => new { it.IngredientId, it.TagId });

        modelBuilder.Entity<IngredientTagRel>()
            .HasOne(it => it.Ingredient)
            .WithMany(i => i.IngredientTagRels)
            .HasForeignKey(it => it.IngredientId);

        modelBuilder.Entity<IngredientTagRel>()
            .HasOne(it => it.Tag)
            .WithMany(t => t.IngredientTagRels)
            .HasForeignKey(it => it.TagId);

    }
}
