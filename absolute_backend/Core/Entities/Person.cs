public class Person
{
    public int Id { get; set; }        // TMDb Person Id
    public string Name { get; set; } = "";
    public string ProfilePath { get; set; } = "";

    public ICollection<Credit> Credits { get; set; } = new List<Credit>();
}
