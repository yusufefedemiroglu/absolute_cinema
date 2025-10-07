namespace Core.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }

    //one product can have one title
    public int? TitleId { get; set; }
}
