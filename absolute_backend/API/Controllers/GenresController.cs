using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly TmdbService _tmdb;

    public GenresController(TmdbService tmdb)
    {
        _tmdb = tmdb;
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportGenres()
    {
        var genres = await _tmdb.ImportGenresAsync();
        return Ok(new
        {
            Message = $"{genres.Count} genres imported.",
            Genres = genres
        });
    }
}
