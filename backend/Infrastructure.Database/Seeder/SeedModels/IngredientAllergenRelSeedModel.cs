using System;
using Domain;

namespace Infrastructure.Database.Seeder.SeedModels;

public class IngredientAllergenRelSeedModel
{
    public Guid IngredientId { get; set; }
    public Guid AllergenId { get; set; }
}
