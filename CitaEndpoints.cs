using BarberappAPI.DTOs;
using BarberappAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BarberappAPI.Endpoints
{
    public static class CitaEndpoints
    {
        public static void MapCitaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/citas")
                .WithTags("Citas")
                .WithOpenApi();

            // POST /api/citas - Crear cita (solo clientes)
            group.MapPost("/", [Authorize(Roles = "CLIENTE")] async (
                [FromBody] CrearCitaDto crearCitaDto,
                [FromServices] ICitaService citaService,
                ClaimsPrincipal user) =>
            {
                if (string.IsNullOrEmpty(crearCitaDto.TipoCorte))
                {
                    return Results.BadRequest(new { message = "El tipo de corte es requerido" });
                }

                var clienteId = long.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var resultado = await citaService.CrearCitaAsync(clienteId, crearCitaDto);

                if (resultado.Success)
                {
                    return Results.Ok(new
                    {
                        message = resultado.Message,
                        cita = resultado.Cita
                    });
                }

                return Results.BadRequest(new { message = resultado.Message });
            })
            .WithName("CrearCita")
            .WithSummary("Crear nueva cita")
            .WithDescription("Permite a un cliente crear una nueva cita")
            .Produces<object>(200)
            .Produces<object>(400)
            .Produces(401);

            // GET /api/citas/mis-citas - Ver mis citas (solo clientes)
            group.MapGet("/mis-citas", [Authorize(Roles = "CLIENTE")] async (
                [FromServices] ICitaService citaService,
                ClaimsPrincipal user) =>
            {
                var clienteId = long.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var resultado = await citaService.ObtenerCitasClienteAsync(clienteId);

                if (resultado.Success)
                {
                    return Results.Ok(new
                    {
                        message = resultado.Message,
                        citas = resultado.Citas
                    });
                }

                return Results.BadRequest(new { message = resultado.Message });
            })
            .WithName("ObtenerMisCitas")
            .WithSummary("Obtener mis citas")
            .WithDescription("Obtiene todas las citas del cliente autenticado")
            .Produces<object>(200)
            .Produces<object>(400)
            .Produces(401);

            // GET /api/citas - Ver todas las citas (solo barberos)
            group.MapGet("/", [Authorize(Roles = "BARBERO")] async (
                [FromServices] ICitaService citaService) =>
            {
                var resultado = await citaService.ObtenerTodasLasCitasAsync();

                if (resultado.Success)
                {
                    return Results.Ok(new
                    {
                        message = resultado.Message,
                        citas = resultado.Citas
                    });
                }

                return Results.BadRequest(new { message = resultado.Message });
            })
            .WithName("ObtenerTodasLasCitas")
            .WithSummary("Obtener todas las citas")
            .WithDescription("Permite a un barbero ver todas las citas del sistema")
            .Produces<object>(200)
            .Produces<object>(400)
            .Produces(401)
            .Produces(403);

            // GET /api/citas/{id} - Obtener cita por ID
            group.MapGet("/{id:long}", [Authorize] async (
                long id,
                [FromServices] ICitaService citaService) =>
            {
                var resultado = await citaService.ObtenerCitaPorIdAsync(id);

                if (resultado.Success)
                {
                    return Results.Ok(new
                    {
                        message = resultado.Message,
                        cita = resultado.Cita
                    });
                }

                return Results.NotFound(new { message = resultado.Message });
            })
            .WithName("ObtenerCitaPorId")
            .WithSummary("Obtener cita por ID")
            .WithDescription("Obtiene una cita específica por su ID")
            .Produces<object>(200)
            .Produces<object>(404)
            .Produces(401);

            // PUT /api/citas/{id}/estado - Actualizar estado (solo barberos)
            group.MapPut("/{id:long}/estado", [Authorize(Roles = "BARBERO")] async (
                long id,
                [FromBody] ActualizarEstadoCitaDto actualizarDto,
                [FromServices] ICitaService citaService) =>
            {
                if (string.IsNullOrEmpty(actualizarDto.Estado))
                {
                    return Results.BadRequest(new { message = "El estado es requerido" });
                }

                var resultado = await citaService.ActualizarEstadoCitaAsync(id, actualizarDto);

                if (resultado.Success)
                {
                    return Results.Ok(new { message = resultado.Message });
                }

                return Results.BadRequest(new { message = resultado.Message });
            })
            .WithName("ActualizarEstadoCita")
            .WithSummary("Actualizar estado de cita")
            .WithDescription("Permite a un barbero actualizar el estado de una cita")
            .Produces<object>(200)
            .Produces<object>(400)
            .Produces(401)
            .Produces(403);

            // DELETE /api/citas/{id} - Eliminar cita (solo barberos)
            group.MapDelete("/{id:long}", [Authorize(Roles = "BARBERO")] async (
                long id,
                [FromServices] ICitaService citaService) =>
            {
                var resultado = await citaService.EliminarCitaAsync(id);

                if (resultado.Success)
                {
                    return Results.Ok(new { message = resultado.Message });
                }

                return Results.BadRequest(new { message = resultado.Message });
            })
            .WithName("EliminarCita")
            .WithSummary("Eliminar cita")
            .WithDescription("Permite a un barbero eliminar una cita")
            .Produces<object>(200)
            .Produces<object>(400)
            .Produces(401)
            .Produces(403);
        }
    }
}
