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
        public async Task<List<Title>> SearchAsync(string query)
        {
            var all = await _repo.GetAllAsync();
            return all
                .Where(t => t.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
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
        public async Task<Title?> GetByTmdbIdAsync(int tmdbId)
        {
            return await _repo.Query()
                .WithFullDetails()
                .FirstOrDefaultAsync(t => t.TmdbId == tmdbId);
        }

        // get by local DB ID
        public async Task<Title?> GetByLocalIdAsync(int id)
        {
            return await _repo.Query()
                .WithFullDetails()
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}