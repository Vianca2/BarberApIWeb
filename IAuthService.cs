using barberiaApp.Models;

namespace barberiaApp.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginDto loginDto);
        Task<bool> RegistroAsync(RegistroUsuarioDto registroDto);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
    }
}
