namespace VetSuccess.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IClientRepository Clients { get; }
    IPatientRepository Patients { get; }
    IPracticeRepository Practices { get; }
    ISMSHistoryRepository SMSHistories { get; }
    IRepository<T> Repository<T>() where T : class;
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
