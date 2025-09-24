public class Genre
{
    public int Id { get; set; }    // Tmdb Genre Id
    public string Name { get; set; } = "";

    public ICollection<TitleGenre> TitleGenres { get; set; } = new List<TitleGenre>();
}
