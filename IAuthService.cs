using BarberappAPI.DTOs;

namespace BarberappAPI.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, UsuarioDto? Usuario)> RegistrarUsuarioAsync(RegistroUsuarioDto registroDto);
        Task<(bool Success, string Message, UsuarioDto? Usuario, string? Token)> LoginAsync(LoginDto loginDto);
        string GenerarToken(UsuarioDto usuario);
    }
}