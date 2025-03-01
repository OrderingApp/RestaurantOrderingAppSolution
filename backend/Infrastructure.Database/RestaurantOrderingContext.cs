using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Database;

public class RestaurantOrderingContextFactory : IDesignTimeDbContextFactory<RestaurantOrderingContext>
{
    public RestaurantOrderingContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RestaurantOrderingContext>();
        optionsBuilder.UseSqlite("YourConnectionStringHere"); // Replace with dynamic configuration

        return new RestaurantOrderingContext(optionsBuilder.Options);
    }
}

public class RestaurantOrderingContext : DbContext
{
    public RestaurantOrderingContext(DbContextOptions<RestaurantOrderingContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<MenuCategory> MenuCategories { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<MenuItemIngredientRel> MenuItemIngredientRels { get; set; }
    public DbSet<IngredientTagRel> IngredientTagRels { get; set; }
    public DbSet<OrderItemIngredient> OrderItemIngredients { get; set; }
    public DbSet<CustomerInformation> CustomerInformations { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<MenuItemSale> MenuItemSales { get; set; }
    public DbSet<SalesRevenue> SalesRevenues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ Enum Conversions
        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion(
                os => os.ToString(),
                os => (OrderStatus)Enum.Parse(typeof(OrderStatus), os)
            );

        modelBuilder.Entity<Order>()
            .Property(o => o.Type)
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
            .Property(t => t.Status)
            .HasConversion(
                ts => ts.ToString(),
                ts => (TableStatus)Enum.Parse(typeof(TableStatus), ts)
            );

        modelBuilder.Entity<CustomerInformation>()
            .Property(ci => ci.OrderCompletionType)
            .HasConversion(
                oc => oc.ToString(),
                oc => (OrderCompletionType)Enum.Parse(typeof(OrderCompletionType), oc)
            );

        modelBuilder.Entity<CustomerInformation>()
            .Property(ci => ci.PreferredPaymentMethod)
            .HasConversion(
                pm => pm.ToString(),
                pm => (PreferredPaymentMethod)Enum.Parse(typeof(PreferredPaymentMethod), pm)
            );

        // ✅ Order and OrderItem relationship (One-to-Many)
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ One-to-One: Order and CustomerInformation
        modelBuilder.Entity<Order>()
            .HasOne(o => o.CustomerInformation)
            .WithOne(ci => ci.Order)
            .HasForeignKey<CustomerInformation>(ci => ci.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ One-To-Many: Order and Payments
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Payments)
            .WithOne(p => p.Order)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // ✅ OrderItem and MenuItem relationship (Many-to-One)
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.MenuItem)
            .WithMany()
            .HasForeignKey(oi => oi.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

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
            .HasForeignKey(mi => mi.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MenuItemIngredientRel>()
            .HasOne(mi => mi.Ingredient)
            .WithMany(i => i.MenuItemIngredientRels)
            .HasForeignKey(mi => mi.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ Many-to-Many: Ingredient and Tag
        modelBuilder.Entity<IngredientTagRel>()
            .HasKey(it => new { it.IngredientId, it.TagId });

        modelBuilder.Entity<IngredientTagRel>()
            .HasOne(it => it.Ingredient)
            .WithMany(i => i.IngredientTagRels)
            .HasForeignKey(it => it.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<IngredientTagRel>()
            .HasOne(it => it.Tag)
            .WithMany(t => t.IngredientTagRels)
            .HasForeignKey(it => it.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ OrderItemIngredient as an owned type (Embedded inside OrderItem)
        modelBuilder.Entity<OrderItem>()
            .OwnsMany(oi => oi.ExtraIngredients, extra =>
            {
                extra.WithOwner().HasForeignKey("OrderItemId");
                extra.Property(i => i.Name).IsRequired();
                extra.Property(i => i.Price).HasColumnType("decimal(18,2)");
                extra.Property(i => i.Quantity).HasDefaultValue(1);
            });

        modelBuilder.Entity<OrderItem>()
            .OwnsMany(oi => oi.RemovedIngredients, removed =>
            {
                removed.WithOwner().HasForeignKey("OrderItemId");
                removed.Property(i => i.Name).IsRequired();
                removed.Property(i => i.Price).HasColumnType("decimal(18,2)");
                removed.Property(i => i.Quantity).HasDefaultValue(1);
            });
    }
}