using Application.Abstractions.Caching;
using Application.DTOs.Titles;
using Application.Services.Queries;
using AutoMapper;
using Core;
using Infrastructure.Data.Repositories.Abstract;
using Infrastructure.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class TitleService : BaseService<Title>
    {
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public TitleService(IGenericRepository<Title> repo, IUnitOfWork uow, IMapper mapper, ILogger<BaseService<Title>> logger, ICacheService cache)
            : base(repo, uow, logger)
        {
            _mapper = mapper;
            _cache = cache;
        }

        // SEARCH
        public async Task<List<TitleLiteDto>> SearchAsync(string query)
        {
            query = query.ToLower().Trim();

            var titles = await _repo.Query()
                .Include(t => t.TitleGenres).ThenInclude(g => g.Genre)
                .Where(t => t.Name.ToLower().Contains(query))
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<TitleLiteDto>>(titles);
        }

        // FULL DETAILS
        public async Task<List<TitleDetailDto>> GetAllWithDetailsAsync()
        {
            var titles = await _repo.Query()
                .WithFullDetails()
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<TitleDetailDto>>(titles);
        }

        // LITE LIST (homepage)
        public async Task<List<TitleLiteDto>> GetAllLiteAsync()
        {
            string cacheKey = "titles-lite";

            // 1) try get from cache
            var cached = await _cache.GetAsync<List<TitleLiteDto>>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("CACHE HIT: titles-lite");
                return cached;
            }

            // 2) not in cache, get from DB
            var titles = await _repo.Query()
                .WithGenres()
                .AsNoTracking()
                .ToListAsync();

            var mapped = _mapper.Map<List<TitleLiteDto>>(titles);

            // 3) add to cache(30 mins)
            await _cache.SetAsync(cacheKey, mapped, TimeSpan.FromMinutes(30));

            _logger.LogInformation("CACHE MISS → DB'den çekildi ve cache'e eklendi: titles-lite");

            return mapped;
        }

        // GET BY TMDB ID
        public async Task<TitleReadDto?> GetByTmdbIdAsync(int tmdbId)
        {
            var title = await _repo.Query()
                .Include(t => t.TitleGenres).ThenInclude(g => g.Genre)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TmdbId == tmdbId);

            return _mapper.Map<TitleReadDto?>(title);
        }

        // GET BY LOCAL ID
        public async Task<TitleReadDto?> GetByLocalIdAsync(int id)
        {
            var title = await _repo.Query()
                .Include(t => t.TitleGenres).ThenInclude(g => g.Genre)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            return _mapper.Map<TitleReadDto?>(title);
        }
    }
}