using AutoMapper;
using BarberappAPI.Data;
using BarberappAPI.DTOs;
using BarberappAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BarberappAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly BarberiaDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(BarberiaDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message, UsuarioDto? Usuario)> RegistrarUsuarioAsync(RegistroUsuarioDto registroDto)
        {
            try
            {
                // Verificar si el email ya existe
                var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == registroDto.Email);
                if (usuarioExistente != null)
                {
                    return (false, "El email ya está registrado", null);
                }

                // Validar rol
                if (registroDto.Rol != "CLIENTE" && registroDto.Rol != "BARBERO")
                {
                    return (false, "Rol inválido", null);
                }

                // Crear nuevo usuario
                var nuevoUsuario = new Usuario
                {
                    Nombre = registroDto.Nombre,
                    Email = registroDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(registroDto.Password),
                    Telefono = registroDto.Telefono,
                    Rol = registroDto.Rol,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                var usuarioDto = _mapper.Map<UsuarioDto>(nuevoUsuario);
                return (true, "Usuario registrado exitosamente", usuarioDto);
            }
            catch (Exception ex)
            {
                return (false, $"Error al registrar usuario: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, UsuarioDto? Usuario, string? Token)> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if (usuario == null)
                {
                    return (false, "Credenciales inválidas", null, null);
                }

                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.Password))
                {
                    return (false, "Credenciales inválidas", null, null);
                }

                var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
                var token = GenerarToken(usuarioDto);

                return (true, "Login exitoso", usuarioDto, token);
            }
            catch (Exception ex)
            {
                return (false, $"Error en login: {ex.Message}", null, null);
            }
        }

        public string GenerarToken(UsuarioDto usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Rol)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}