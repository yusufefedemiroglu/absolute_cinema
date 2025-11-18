namespace Application.DTOs.Titles
{
    public class TitleDetailDto
    {
        public int Id { get; set; } //Local db id
        public int TmdbId { get; set; } //TMDB id
        public string Name { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public DateTime? ReleaseDate { get; set; }
        public string PosterPath { get; set; } = string.Empty;
        public double VoteAverage { get; set; }
        public string Type { get; set; } = string.Empty;
        public List<string> Genres { get; set; } = new();
    }
}