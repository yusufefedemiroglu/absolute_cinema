using Core.DTOs.Tmdb;
using Core;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure.Services;

public class TmdbService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly AppDbContext _db;

    public TmdbService(HttpClient httpClient, IConfiguration config, AppDbContext db)
    {
        _httpClient = httpClient;
        _config = config;
        _db = db;
        _httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");
    }

    public async Task<List<Title>> ImportPopularMoviesAsync()
    {
        var apiKey = _config["TMDb:ApiKey"];
        var response = await _httpClient.GetFromJsonAsync<TmdbMovieResponse>(
            $"movie/popular?api_key={apiKey}&language=en-US&page=1"
        );

        if (response == null || response.Results.Count == 0)
            return new List<Title>();

        var movies = new List<Title>();
        foreach (var r in response.Results)
        {

            // db check
            var exists = await _db.Titles.AnyAsync(t => t.TmdbId == r.Id);
            if (!exists)
            {
                Console.WriteLine($"Movie: {r.Title}, ReleaseDate(raw): {r.ReleaseDate}");
                var title = new Title
                {
                    TmdbId = r.Id,
                    Name = r.Title,
                    Overview = r.Overview,
                    ReleaseDate = string.IsNullOrEmpty(r.ReleaseDate)
    ? null
    : DateTime.ParseExact(
        r.ReleaseDate,
        "yyyy-MM-dd",
        CultureInfo.InvariantCulture
      ),


                    PosterPath = r.PosterPath,
                    Type = "movie"
                };

                movies.Add(title);
                _db.Titles.Add(title);
            }
        }

        await _db.SaveChangesAsync();
        return movies;
    }
    public async Task<List<TmdbMovieDto>> FetchPopularMoviesRawAsync()
    {
        var apiKey = _config["TMDb:ApiKey"];
        var response = await _httpClient.GetFromJsonAsync<TmdbMovieResponse>(
            $"movie/popular?api_key={apiKey}&language=en-US&page=1"
        );

        if (response == null) return new List<TmdbMovieDto>();

        // Loglama
        foreach (var r in response.Results)
        {
            Console.WriteLine($"Movie: {r.Title}, ReleaseDate(raw): {r.ReleaseDate}");
        }

        return response.Results;
    }

}
