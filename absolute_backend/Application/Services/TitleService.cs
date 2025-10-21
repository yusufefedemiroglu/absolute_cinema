using Core;
using Infrastructure.Data;
using Infrastructure.Data.Repositories.Abstract;
using Infrastructure.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class TitleService : BaseService<Title>
    {
        public TitleService(IGenericRepository<Title> repository, IUnitOfWork unitOfWork)
            : base(repository, unitOfWork)
        {
        }

        // 🎯 Title’a özel metotlar
        public async Task<List<Title>> SearchAsync(string query)
        {
            var allTitles = await _repo.GetAllAsync();
            return allTitles.Where(t => t.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        public async Task<List<Title>> GetAllWithDetailsAsync(AppDbContext context)
        {
            return await context.Titles
                .Include(t => t.TitleGenres)
                    .ThenInclude(tg => tg.Genre)
                .Include(t => t.Credits)
                    .ThenInclude(c => c.Person)
                .ToListAsync();
        }

    }
}