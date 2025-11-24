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
    private readonly ILogger<TitlesController> _logger;

    public TitlesController(TitleService titleService, TmdbService tmdbService, ILogger<TitlesController> logger)
    {
        _titleService = titleService;
        _tmdbService = tmdbService;
        _logger = logger;
    }

    // LITE (homepage)
    [HttpGet("lite")]
    [ProducesResponseType(typeof(List<TitleLiteDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAllLite()
    {
        var titles = await _titleService.GetAllLiteAsync();

        if (titles.Count == 0)
            return NotFound(new { Message = "No titles available." });

        return Ok(titles);
    }

    // FULL DETAILS
    [HttpGet("with-details")]
    [ProducesResponseType(typeof(List<TitleDetailDto>), 200)]
    public async Task<IActionResult> GetAllWithDetails()
    {
        var titles = await _titleService.GetAllWithDetailsAsync();
        return Ok(titles);
    }

    // LOCAL DB ID
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TitleReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByLocalId(int id)
    {
        var title = await _titleService.GetByLocalIdAsync(id);
        if (title == null)
            return NotFound(new { Message = "Title not found." });

        return Ok(title);
    }

    // TMDB ID
    [HttpGet("tmdb/{tmdbId:int}")]
    [ProducesResponseType(typeof(TitleReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByTmdbId(int tmdbId)
    {
        var title = await _titleService.GetByTmdbIdAsync(tmdbId);
        if (title == null)
            return NotFound(new { Message = "Title not found." });

        return Ok(title);
    }

    // SEARCH
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<TitleLiteDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { Message = "Search query cannot be empty." });

        var results = await _titleService.SearchAsync(query.Trim());

        if (results.Count == 0)
            return NotFound(new { Message = "No titles found." });

        return Ok(results);
    }

    // IMPORT POPULAR
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

    // RAW TMDB (debug)
    [HttpGet("tmdb/raw")]
    public async Task<IActionResult> GetPopularMoviesRaw()
    {
        var movies = await _tmdbService.FetchPopularMoviesRawAsync();
        return Ok(movies);
    }
}