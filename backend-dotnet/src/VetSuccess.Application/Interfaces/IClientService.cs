using VetSuccess.Application.DTOs.Client;

namespace VetSuccess.Application.Interfaces;

public interface IClientService
{
    Task<List<ClientListDto>> SearchClientsAsync(string? search, string? practiceOduId, CancellationToken cancellationToken = default);
    Task<ClientDetailDto> GetClientDetailsAsync(string clientOduId, CancellationToken cancellationToken = default);
    Task<ClientDetailDto> UpdateClientAsync(string clientOduId, ClientUpdateDto updateDto, CancellationToken cancellationToken = default);
}
