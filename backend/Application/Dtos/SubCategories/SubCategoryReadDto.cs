namespace Application.Dtos.SubCategories;

public class SubCategoryReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int TotalItems { get; set; }
}
