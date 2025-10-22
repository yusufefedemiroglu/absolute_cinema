namespace Application.DTOs
{
    public class TitleLiteDto
    {

        public int TmdbId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string PosterPath{ get; set; } = string.Empty;

        public double VoteAverage { get; set; }

        public List<string> Genres { get; set; } = new();
    }
}