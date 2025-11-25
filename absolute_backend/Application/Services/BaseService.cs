using Infrastructure.Data.Repositories.Abstract;
using Infrastructure.Data.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class BaseService<T> where T : class
    {
        protected readonly IGenericRepository<T> _repo;
        protected readonly IUnitOfWork _uow;
        protected readonly ILogger<BaseService<T>> _logger;

        public BaseService(
            IGenericRepository<T> repo,
            IUnitOfWork uow,
            ILogger<BaseService<T>> logger)
        {
            _repo = repo;
            _uow = uow;
            _logger = logger;
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            _logger.LogInformation("Getting all {EntityName}", typeof(T).Name);

            try
            {
                var result = await _repo.GetAllAsync();
                _logger.LogInformation("Successfully retrieved {Count} {EntityName} items",
                    result.Count(), typeof(T).Name);

                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting all {EntityName}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting {EntityName} by Id: {Id}", typeof(T).Name, id);

            try
            {
                var entity = await _repo.GetByIdAsync(id);

                if (entity == null)
                    _logger.LogWarning("{EntityName} not found (Id: {Id})", typeof(T).Name, id);
                else
                    _logger.LogInformation("{EntityName} successfully found (Id: {Id})", typeof(T).Name, id);

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting {EntityName} by Id: {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public virtual async Task AddAsync(T entity)
        {
            _logger.LogInformation("Creating new {EntityName}: {@Entity}", typeof(T).Name, entity);

            try
            {
                await _repo.AddAsync(entity);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("{EntityName} created successfully", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating {EntityName}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _logger.LogInformation("Updating {EntityName}: {@Entity}", typeof(T).Name, entity);

            try
            {
                _repo.Update(entity);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("{EntityName} updated successfully",
                    typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating {EntityName}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task RemoveAsync(T entity)
        {
            _logger.LogInformation("Removing {EntityName}: {@Entity}", typeof(T).Name, entity);

            try
            {
                _repo.Remove(entity);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("{EntityName} removed successfully", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while removing {EntityName}", typeof(T).Name);
                throw;
            }
        }
    }
}