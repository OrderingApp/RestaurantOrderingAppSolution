namespace Domain;

public class MenuItemSale
{
    public Guid Id { get; set; }
    public int Ammount { get; set; }
    public DateTime Date { get; set; }
    public Guid MenuItemId { get; set; }
    public MenuItem? MenuItem { get; set; }
}
