using System.Text.Json.Serialization;

namespace Core.DTOs.Tmdb;

public class TmdbMovieResponse
{
    public List<TmdbMovieDto> Results { get; set; } = new();
}

public class TmdbMovieDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Overview { get; set; } = "";
    public string ReleaseDate { get; set; } = "";
    public string PosterPath { get; set; } = "";
    public string FirstAirDate { get; set; } = "";

    [JsonPropertyName("genre_ids")]
    public List<int> GenreIds { get; set; } = new();

}
