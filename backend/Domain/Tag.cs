namespace Domain;

public class Tag
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsUsed { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public List<IngredientTagRel> IngredientTagRels { get; set; } = new();
}
