using Application.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TitlesController : ControllerBase
{
    private readonly TitleService _titleService;
    private readonly TmdbService _tmdbService;

    public TitlesController(TitleService titleService, TmdbService tmdbService)
    {
        _titleService = titleService;
        _tmdbService = tmdbService;
    }

    // 🔹 DB'deki tüm filmler
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var titles = await _titleService.GetAllAsync();
        return Ok(titles);
    }

    // 🔹 Tek film
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var title = await _titleService.GetByIdAsync(id);
        if (title == null) return NotFound();
        return Ok(title);
    }

    // 🔹 Arama
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var results = await _titleService.SearchAsync(query);
        return Ok(results);
    }

    // 🔹 TMDb'den film import et
    [HttpPost("import/popular")]
    public async Task<IActionResult> ImportPopular()
    {
        var movies = await _tmdbService.ImportPopularMoviesAsync();
        return Ok(new
        {
            Message = $"{movies.Count} movies imported.",
            Movies = movies
        });
    }
    [HttpGet("tmdb/raw")]
    public async Task<IActionResult> GetPopularMoviesRaw()
    {
        var movies = await _tmdbService.FetchPopularMoviesRawAsync();
        return Ok(movies);
    }

}
