using VetSuccess.Domain.Entities;

namespace VetSuccess.Domain.Interfaces;

public interface IPatientRepository : IRepository<Patient>
{
    Task<IEnumerable<Patient>> GetPatientsByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
}
