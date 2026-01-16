using VetSuccess.Application.DTOs.Auth;

namespace VetSuccess.Application.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
