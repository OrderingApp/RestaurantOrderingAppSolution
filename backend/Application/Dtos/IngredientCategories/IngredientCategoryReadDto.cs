using Application.Dtos.Ingredients;

namespace Application.Dtos.IngredientCategories;

public class IngredientCategoryReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public List<IngredientReadDto> Ingredients { get; set; } = new();
}
