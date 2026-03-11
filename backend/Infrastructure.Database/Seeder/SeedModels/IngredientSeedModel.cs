namespace Infrastructure.Database.Seeder.SeedModels;

public class IngredientSeedModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }

    public bool CanBeUsedAsExtra { get; set; }
    public bool IsDeleted { get; set; } = false;

    public Guid? CategoryId { get; set; }
}
