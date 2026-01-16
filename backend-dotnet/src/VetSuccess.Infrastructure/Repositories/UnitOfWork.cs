using Microsoft.EntityFrameworkCore.Storage;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Infrastructure.Data;

namespace VetSuccess.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly VetSuccessDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repositories = new();
    
    private IClientRepository? _clients;
    private IPatientRepository? _patients;
    private IPracticeRepository? _practices;
    private ISMSHistoryRepository? _smsHistories;

    public UnitOfWork(VetSuccessDbContext context)
    {
        _context = context;
    }

    public IClientRepository Clients => _clients ??= new ClientRepository(_context);
    public IPatientRepository Patients => _patients ??= new PatientRepository(_context);
    public IPracticeRepository Practices => _practices ??= new PracticeRepository(_context);
    public ISMSHistoryRepository SMSHistories => _smsHistories ??= new SMSHistoryRepository(_context);

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new GenericRepository<T>(_context);
        }
        return (IRepository<T>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
