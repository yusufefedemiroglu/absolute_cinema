using System.Linq.Expressions;

namespace Infrastructure.Data.Repositories.Abstract;

public interface IGenericRepository<T> where T : class
{
  Task<IEnumerable<T>> GetAllAsync();

  Task<T?> GetByIdAsync(Guid id);
  Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
  Task AddAsync(T entity);
  void Update(T entity);
  void Remove(T entity);
  Task<IEnumerable<T>> GetAllWithIncludeAsync(
    Func<IQueryable<T>, IQueryable<T>> include
);

  IQueryable<T> Query();

}