public class Title
{
    public int Id { get; set; }            // DB PK
    public int TmdbId { get; set; }        // tmdb id
    public string Type { get; set; } = ""; // movie, tv, anime
    public string Name { get; set; } = "";
    public string OriginalName { get; set; } = "";
    public string Overview { get; set; } = "";
    public DateTime? ReleaseDate { get; set; }
    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }
    public string PosterPath { get; set; } = "";
    public string BackdropPath { get; set; } = "";

    public ICollection<Credit> Credits { get; set; } = new List<Credit>();
    public ICollection<TitleGenre> TitleGenres { get; set; } = new List<TitleGenre>();
}
