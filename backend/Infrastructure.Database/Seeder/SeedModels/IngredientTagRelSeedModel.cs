using System;
using Domain;

namespace Infrastructure.Database.Seeder.SeedModels;

public class IngredientTagRelSeedModel
{
    public Guid IngredientId { get; set; }
    public Guid TagId { get; set; }
}
