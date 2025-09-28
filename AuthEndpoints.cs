using BarberappAPI.DTOs;
using BarberappAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarberappAPI.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/auth")
                .WithTags("Autenticación")
                .WithOpenApi();

            // POST /api/auth/registro
            group.MapPost("/registro", async (
                [FromBody] RegistroUsuarioDto registroDto,
                [FromServices] IAuthService authService) =>
            {
                // Validación manual básica
                if (string.IsNullOrEmpty(registroDto.Nombre) ||
                    string.IsNullOrEmpty(registroDto.Email) ||
                    string.IsNullOrEmpty(registroDto.Password))
                {
                    return Results.BadRequest(new { message = "Todos los campos son requeridos" });
                }

                var resultado = await authService.RegistrarUsuarioAsync(registroDto);

                if (resultado.Success)
                {
                    return Results.Ok(new
                    {
                        message = resultado.Message,
                        usuario = resultado.Usuario
                    });
                }

                return Results.BadRequest(new { message = resultado.Message });
            })
            .WithName("RegistrarUsuario")
            .WithSummary("Registrar un nuevo usuario")
            .WithDescription("Registra un nuevo usuario como CLIENTE o BARBERO")
            .Produces<object>(200)
            .Produces<object>(400);

            // POST /api/auth/login
            group.MapPost("/login", async (
                [FromBody] LoginDto loginDto,
                [FromServices] IAuthService authService) =>
            {
                if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                {
                    return Results.BadRequest(new { message = "Email y contraseña son requeridos" });
                }

                var resultado = await authService.LoginAsync(loginDto);

                if (resultado.Success)
                {
                    return Results.Ok(new
                    {
                        message = resultado.Message,
                        usuario = resultado.Usuario,
                        token = resultado.Token
                    });
                }

                return Results.BadRequest(new { message = resultado.Message });
            })
            .WithName("IniciarSesion")
            .WithSummary("Iniciar sesión")
            .WithDescription("Autentica un usuario y devuelve un token JWT")
            .Produces<object>(200)
            .Produces<object>(400);
        }
    }
}