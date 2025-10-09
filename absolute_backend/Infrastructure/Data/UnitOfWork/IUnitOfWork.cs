using Infrastructure.Data.Repositories.Abstract;

namespace Infrastructure.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        /// access the generic repository 
        IGenericRepository<T> Repository<T>() where T : class;


        /// saves all repository changes in a single transaction.

        Task<int> SaveChangesAsync();


        /// starts a new transaction.

        Task BeginTransactionAsync();


        /// transaction commit 

        Task CommitTransactionAsync();


        /// Transaction rollback 

        Task RollbackTransactionAsync();
    }
}