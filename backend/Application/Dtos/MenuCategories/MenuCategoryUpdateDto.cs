namespace Application.Dtos.MenuCategories;

public class MenuCategoryUpdateDto
{
    public string? Name { get; set; }
    public bool IsUsed { get; set; }
    public bool IsDeleted { get; set; }
}