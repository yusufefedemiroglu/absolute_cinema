using System.Text.Json.Serialization;

public class TitleGenre
{

    public int TitleId { get; set; }
    [JsonIgnore]
    public Title Title { get; set; } = null!;

    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
}
