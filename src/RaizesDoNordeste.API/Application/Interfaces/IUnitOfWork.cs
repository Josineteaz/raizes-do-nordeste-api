namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<bool> SaveChangesAsync();
    }
}