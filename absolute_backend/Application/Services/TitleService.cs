using Application.DTOs.Titles;
using Application.Services.Mappers;
using Application.Services.Queries;
using Core;
using Infrastructure.Data.Repositories.Abstract;
using Infrastructure.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class TitleService : BaseService<Title>
    {
        public TitleService(IGenericRepository<Title> repo, IUnitOfWork uow)
            : base(repo, uow) { }

        // search by query
        public async Task<List<TitleLiteDto>> SearchAsync(string query)
        {
            query = query.ToLower().Trim();

            var titles = await _repo.Query()
                .Include(t => t.TitleGenres).ThenInclude(g => g.Genre)
                .Where(t => t.Name.ToLower().Contains(query))
                .ToListAsync();

            return titles.Select(TitleMapper.ToLite).ToList();
        }

        // full detailed list
        public async Task<List<TitleDetailDto>> GetAllWithDetailsAsync()
        {
            var titles = await _repo.Query()
                .WithFullDetails()
                .AsNoTracking()
                .ToListAsync();

            return titles.Select(TitleMapper.ToDetail).ToList();
        }

        // lite homepage optimized list
        public async Task<List<TitleLiteDto>> GetAllLiteAsync()
        {
            var titles = await _repo.Query()
                .WithGenres()
                .AsNoTracking()
                .ToListAsync();

            return titles.Select(TitleMapper.ToLite).ToList();
        }

        // get by TMDb ID 
        public async Task<TitleReadDto?> GetByTmdbIdAsync(int tmdbId)
        {
            var title = await _repo.Query()
                .Include(t => t.TitleGenres).ThenInclude(g => g.Genre)
                .FirstOrDefaultAsync(t => t.TmdbId == tmdbId);

            return title == null ? null : TitleMapper.ToRead(title);
        }
        // get by local DB ID to dto
        public async Task<TitleReadDto?> GetByLocalIdAsync(int id)
        {
            var title = await _repo.Query()
                .Include(t => t.TitleGenres).ThenInclude(g => g.Genre)
                .FirstOrDefaultAsync(t => t.Id == id);

            return title == null ? null : TitleMapper.ToRead(title);
        }


    }
}