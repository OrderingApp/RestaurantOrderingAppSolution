namespace Application.Dtos.SubCategories;

public class SubCategoryCreateDto
{
    public string Name { get; set; } = null!;
    public Guid MenuCategoryId { get; set; }
}