using Core;
using Infrastructure.Data.Repositories.Abstract;
using Infrastructure.Data.UnitOfWork;

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
    }
}