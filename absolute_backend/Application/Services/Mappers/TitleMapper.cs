using Application.DTOs.Titles;
using Core;

namespace Application.Services.Mappers
{
    public static class TitleMapper
    {
        // base URL for TMDb images
        private const string ImageBase = "https://image.tmdb.org/t/p/w500";

        private static string BuildPoster(string? path)
            => string.IsNullOrWhiteSpace(path) ? string.Empty : $"{ImageBase}{path}";

        public static TitleLiteDto ToLite(Title t)
        {
            return new TitleLiteDto
            {
                TmdbId = t.TmdbId,
                Name = t.Name,
                PosterPath = BuildPoster(t.PosterPath),
                VoteAverage = t.VoteAverage,
                Genres = t.TitleGenres.Select(g => g.Genre.Name).ToList()
            };
        }

        public static TitleDetailDto ToDetail(Title t)
        {
            return new TitleDetailDto
            {
                Id = t.Id,
                TmdbId = t.TmdbId,
                Name = t.Name,
                Overview = t.Overview ?? string.Empty,
                ReleaseDate = t.ReleaseDate,
                PosterPath = BuildPoster(t.PosterPath),
                VoteAverage = t.VoteAverage,
                Type = t.Type ?? "",
                Genres = t.TitleGenres.Select(g => g.Genre.Name).ToList()
            };
        }
        public static TitleReadDto ToRead(Title t)
        {
            return new TitleReadDto
            {
                Id = t.Id,
                TmdbId = t.TmdbId,
                Name = t.Name,
                Overview = t.Overview ?? "",
                PosterPath = BuildPoster(t.PosterPath),
                VoteAverage = t.VoteAverage,
                Type = t.Type ?? ""
            };
        }
    }
}