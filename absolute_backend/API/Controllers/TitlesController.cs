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
        if (!titles.Any())
            return NotFound(new { Message = "No titles found." });

        return Ok(titles);
    }

    // Detailed version
    [HttpGet("with-details")]
    [ProducesResponseType(typeof(List<TitleDetailDto>), 200)]
    public async Task<ActionResult<List<TitleDetailDto>>> GetAllWithDetails()
    {
        var titles = await _titleService.GetAllWithDetailsAsync();
        return Ok(titles);
    }

    //  Single title
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var title = await _titleService.GetByIdAsync(id);
        if (title == null) return NotFound();
        return Ok(title);
    }

    // Search
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var results = await _titleService.SearchAsync(query);
        return Ok(results);
    }

    // mport popular from TMDb
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

    //Raw TMDb (debug/test)
    [HttpGet("tmdb/raw")]
    public async Task<IActionResult> GetPopularMoviesRaw()
    {
        var movies = await _tmdbService.FetchPopularMoviesRawAsync();
        return Ok(movies);
    }
}