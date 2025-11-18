using Application.DTOs;
using Application.DTOs.Titles;
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

    // LITE – Homepage optimized
    [HttpGet("lite")]
    [ProducesResponseType(typeof(List<TitleLiteDto>), 200)]
    public async Task<ActionResult<List<TitleLiteDto>>> GetAllLite()
    {
        var titles = await _titleService.GetAllLiteAsync();
        return Ok(titles);
    }

    // Detailed list
    [HttpGet("with-details")]
    [ProducesResponseType(typeof(List<TitleDetailDto>), 200)]
    public async Task<ActionResult<List<TitleDetailDto>>> GetAllWithDetails()
    {
        var titles = await _titleService.GetAllWithDetailsAsync();
        return Ok(titles);
    }

    // Single Title (LOCAL DB ID)
    [HttpGet("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByLocalId(int id)
    {
        var title = await _titleService.GetByLocalIdAsync(id);
        if (title == null)
            return NotFound(new { Message = "Title not found." });

        return Ok(title);
    }

    // Single Title (TMDb ID)
    [HttpGet("tmdb/{tmdbId:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByTmdbId(int tmdbId)
    {
        var title = await _titleService.GetByTmdbIdAsync(tmdbId);
        if (title == null)
            return NotFound(new { Message = "Title not found." });

        return Ok(title);
    }

    // Search
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var results = await _titleService.SearchAsync(query);
        return Ok(results);
    }

    // Import popular movies from TMDb
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

    // Raw TMDb data
    [HttpGet("tmdb/raw")]
    public async Task<IActionResult> GetPopularMoviesRaw()
    {
        var movies = await _tmdbService.FetchPopularMoviesRawAsync();
        return Ok(movies);
    }
}