using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Genre
{
    // not autoinc 
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    [JsonIgnore]
    public ICollection<TitleGenre> TitleGenres { get; set; } = new List<TitleGenre>();
}
