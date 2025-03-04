namespace Application.Dtos.Areas;

public class AreaUpdateDto
{
    public required string Name { get; set; }
    public bool IsUsed { get; set; }
    public bool IsDeleted { get; set; }
}