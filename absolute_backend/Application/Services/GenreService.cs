using Core;
using Infrastructure.Data.Repositories.Abstract;
using Infrastructure.Data.UnitOfWork;

namespace Application.Services
{
    public class GenreService : BaseService<Genre>
    {
        public GenreService(IGenericRepository<Genre> repository, IUnitOfWork unitOfWork, ILogger<BaseService<Genre>> logger)
            : base(repository, unitOfWork, logger)
        {
        }
    }
}