using VetSuccess.Domain.Entities;

namespace VetSuccess.Domain.Interfaces;

public interface IClientRepository : IRepository<Client>
{
    Task<Client?> GetClientWithDetailsAsync(string oduId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Client>> SearchClientsAsync(string? searchTerm, string? practiceOduId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Client>> SearchClientsByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default);
}
