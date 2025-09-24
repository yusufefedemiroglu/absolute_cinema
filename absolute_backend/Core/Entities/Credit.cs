public class Credit
{
    public int Id { get; set; }
    public int TitleId { get; set; }
    public Title Title { get; set; } = null!;

    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;

    public string Role { get; set; } = "";       // Cast/Crew
    public string? Character { get; set; }       // character name for cast
    public string? Department { get; set; }      // directing,  writing
    public int Order { get; set; }               //  cast order
}
