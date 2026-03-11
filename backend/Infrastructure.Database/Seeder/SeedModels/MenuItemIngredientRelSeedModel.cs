using System;
using Domain;

namespace Infrastructure.Database.Seeder.SeedModels;

public class MenuItemIngredientRelSeedModel
{
    public Guid MenuItemId { get; set; }
    public Guid IngredientId { get; set; }
}
