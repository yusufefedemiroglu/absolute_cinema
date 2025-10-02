using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Genre
{
    // not autoinc 
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<TitleGenre> TitleGenres { get; set; } = new List<TitleGenre>();
}
