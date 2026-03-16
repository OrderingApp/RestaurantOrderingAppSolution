using System;
using System.Threading.Tasks;

namespace Infrastructure.Database.Seeder.Seeders;

/// <summary>
/// Orchestrates all JSON-based seeders in the correct dependency order.
/// Data source: JSON files in Infrastructure.Database/Seeder/SeedData/
/// Seeders operate in "if empty" mode – they are safe to call on
/// every startup but will not overwrite existing data.
/// </summary>
public class DatabaseSeeder
{
    private readonly MenuCategorySeeder _menuCategorySeeder;
    private readonly SubCategorySeeder _subCategorySeeder;
    private readonly IngredientCategorySeeder _ingredientCategorySeeder;
    private readonly TagSeeder _tagSeeder;
    private readonly AllergenSeeder _allergenSeeder;
    private readonly AreaSeeder _areaSeeder;
    private readonly IngredientSeeder _ingredientSeeder;
    private readonly TableSeeder _tableSeeder;
    private readonly MenuItemSeeder _menuItemSeeder;
    private readonly IngredientTagRelSeeder _ingredientTagRelSeeder;
    private readonly IngredientAllergenRelSeeder _ingredientAllergenRelSeeder;
    private readonly MenuItemIngredientRelSeeder _menuItemIngredientRelSeeder;

    public DatabaseSeeder(
        MenuCategorySeeder menuCategorySeeder,
        SubCategorySeeder subCategorySeeder,
        IngredientCategorySeeder ingredientCategorySeeder,
        TagSeeder tagSeeder,
        AllergenSeeder allergenSeeder,
        AreaSeeder areaSeeder,
        IngredientSeeder ingredientSeeder,
        TableSeeder tableSeeder,
        MenuItemSeeder menuItemSeeder,
        IngredientTagRelSeeder ingredientTagRelSeeder,
        IngredientAllergenRelSeeder ingredientAllergenRelSeeder,
        MenuItemIngredientRelSeeder menuItemIngredientRelSeeder)
    {
        _menuCategorySeeder = menuCategorySeeder;
        _subCategorySeeder = subCategorySeeder;
        _ingredientCategorySeeder = ingredientCategorySeeder;
        _tagSeeder = tagSeeder;
        _allergenSeeder = allergenSeeder;
        _areaSeeder = areaSeeder;
        _ingredientSeeder = ingredientSeeder;
        _tableSeeder = tableSeeder;
        _menuItemSeeder = menuItemSeeder;
        _ingredientTagRelSeeder = ingredientTagRelSeeder;
        _ingredientAllergenRelSeeder = ingredientAllergenRelSeeder;
        _menuItemIngredientRelSeeder = menuItemIngredientRelSeeder;
    }

    public async Task SeedAsync()
    {
        // Order matters: parent entities must be seeded before children.
        // Base entities
        await _menuCategorySeeder.SeedAsync();
        await _subCategorySeeder.SeedAsync();
        await _ingredientCategorySeeder.SeedAsync();
        await _tagSeeder.SeedAsync();
        await _allergenSeeder.SeedAsync();
        await _areaSeeder.SeedAsync();
        await _ingredientSeeder.SeedAsync();
        await _tableSeeder.SeedAsync();
        await _menuItemSeeder.SeedAsync();

        // Relational join tables (depend on all base entities above)
        await _ingredientTagRelSeeder.SeedAsync();
        await _ingredientAllergenRelSeeder.SeedAsync();
        await _menuItemIngredientRelSeeder.SeedAsync();
    }
}
