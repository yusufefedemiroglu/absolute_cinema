using Application.DTOs;
using Application.DTOs.Titles;
using Core;
using Infrastructure.Data.Repositories.Abstract;
using Infrastructure.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class TitleService : BaseService<Title>
    {
        private const string ImageBase = "https://image.tmdb.org/t/p/w500";

        public TitleService(IGenericRepository<Title> repository, IUnitOfWork unitOfWork)
            : base(repository, unitOfWork) { }

        // in memory search (for demo purposes)
        public async Task<List<Title>> SearchAsync(string query)
        {
            var allTitles = await _repo.GetAllAsync();
            return allTitles
                .Where(t => t.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private IQueryable<Title> IncludeGenres(IQueryable<Title> query)
        {
            return query
                .Include(t => t.TitleGenres)
                .ThenInclude(tg => tg.Genre);
        }
        // detailed list
        public async Task<List<TitleDetailDto>> GetAllWithDetailsAsync()
        {
            var titles = await _repo.GetAllWithIncludeAsync(IncludeGenres);

            return titles.Select(t => new TitleDetailDto
            {
                Id = t.Id,
                TmdbId = t.TmdbId,
                Name = t.Name,
                Overview = t.Overview ?? string.Empty,
                ReleaseDate = t.ReleaseDate,
                PosterPath = BuildPosterUrl(t.PosterPath),
                VoteAverage = t.VoteAverage,
                Type = t.Type ?? string.Empty,
                Genres = t.TitleGenres.Select(g => g.Genre.Name).ToList()
            }).ToList();
        }

        // lite list for homepage
        public async Task<List<TitleLiteDto>> GetAllLiteAsync()
        {
            var titles = await _repo.GetAllWithIncludeAsync(IncludeGenres);

            return titles.Select(t => new TitleLiteDto
            {
                TmdbId = t.TmdbId,
                Name = t.Name,
                PosterPath = BuildPosterUrl(t.PosterPath),
                VoteAverage = t.VoteAverage,
                Genres = t.TitleGenres.Select(g => g.Genre.Name).ToList()
            }).ToList();
        }

        // little helper
        private static string BuildPosterUrl(string? posterPath)
            => string.IsNullOrWhiteSpace(posterPath) ? string.Empty : $"{ImageBase}{posterPath}";
    }
}