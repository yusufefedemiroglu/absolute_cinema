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

        public TitleService(IGenericRepository<Title> repo, IUnitOfWork uow, IMapper mapper, ILogger<BaseService<Title>> logger)
            : base(repo, uow, logger)
        {
            _mapper = mapper;
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
            var titles = await _repo.Query()
                .WithGenres()
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<TitleLiteDto>>(titles);
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