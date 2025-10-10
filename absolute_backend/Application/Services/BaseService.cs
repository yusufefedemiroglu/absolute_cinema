using Infrastructure.Data.Repositories.Abstract;
using Infrastructure.Data.UnitOfWork;

namespace Application.Services
{
    public class BaseService<T> where T : class
    {
        protected readonly IGenericRepository<T> _repo;
        protected readonly IUnitOfWork _uow;

        public BaseService(IGenericRepository<T> repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            var result = await _repo.GetAllAsync();
            return result.ToList();
        }

        public virtual async Task<T?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public virtual async Task AddAsync(T entity)
        {
            await _repo.AddAsync(entity);
            await _uow.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _repo.Update(entity);
            await _uow.SaveChangesAsync();
        }

        public virtual async Task RemoveAsync(T entity)
        {
            _repo.Remove(entity);
            await _uow.SaveChangesAsync();
        }
    }
}