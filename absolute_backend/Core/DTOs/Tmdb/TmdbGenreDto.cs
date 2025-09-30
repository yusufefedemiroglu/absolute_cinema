namespace Core.DTOs;

public class TmdbGenreDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TmdbGenreResponse
{
    public List<TmdbGenreDto> Genres { get; set; } = new();
}
