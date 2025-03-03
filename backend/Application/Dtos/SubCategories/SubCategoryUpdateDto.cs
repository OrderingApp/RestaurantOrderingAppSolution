namespace Application.Dtos.SubCategories;

public class SubCategoryUpdateDto
{
    public string? Name { get; set; }
    public bool? IsUsed { get; set; }
    public bool? IsDeleted { get; set; }
    public Guid? MenuCategoryId { get; set; }
}