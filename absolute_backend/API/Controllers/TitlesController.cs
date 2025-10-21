using Application.Services;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TitlesController : ControllerBase
{
    private readonly TitleService _titleService;
    private readonly TmdbService _tmdbService;

    private readonly AppDbContext _context;

    public TitlesController(TitleService titleService, TmdbService tmdbService, AppDbContext context)
    {
        _titleService = titleService;
        _tmdbService = tmdbService;
        _context = context;
    }

    [HttpGet("with-details")]
    public async Task<IActionResult> GetAllWithDetails([FromServices] AppDbContext context)
    {
        var titles = await _titleService.GetAllWithDetailsAsync(context);
        return Ok(titles);
    }

    // 🔹 One Movie
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var title = await _titleService.GetByIdAsync(id);
        if (title == null) return NotFound();
        return Ok(title);
    }

    // 🔹 Search
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var results = await _titleService.SearchAsync(query);
        return Ok(results);
    }

    // 🔹 Import 
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
