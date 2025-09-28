using AutoMapper;
using BarberappAPI.Data;
using BarberappAPI.DTOs;
using BarberappAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberappAPI.Services
{
    public class CitaService : ICitaService
    {
        private readonly BarberiaDbContext _context;
        private readonly IMapper _mapper;

        public CitaService(BarberiaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<(bool Success, string Message, CitaDto? Cita)> CrearCitaAsync(long clienteId, CrearCitaDto crearCitaDto)
        {
            try
            {
                // Verificar que el cliente existe
                var cliente = await _context.Usuarios.FindAsync(clienteId);
                if (cliente == null)
                {
                    return (false, "Cliente no encontrado", null);
                }

                // Verificar que la fecha no sea en el pasado
                if (crearCitaDto.FechaHora <= DateTime.Now)
                {
                    return (false, "La fecha debe ser en el futuro", null);
                }

                // Verificar disponibilidad (opcional: evitar citas duplicadas en el mismo horario)
                var citaExistente = await _context.Citas
                    .Where(c => c.FechaHora == crearCitaDto.FechaHora &&
                               (c.Estado == "PENDIENTE" || c.Estado == "CONFIRMADA"))
                    .FirstOrDefaultAsync();

                if (citaExistente != null)
                {
                    return (false, "Ya existe una cita en este horario", null);
                }

                var nuevaCita = new Cita
                {
                    ClienteId = clienteId,
                    FechaHora = crearCitaDto.FechaHora,
                    TipoCorte = crearCitaDto.TipoCorte,
                    Descripcion = crearCitaDto.Descripcion,
                    Estado = "PENDIENTE",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Citas.Add(nuevaCita);
                await _context.SaveChangesAsync();

                // Cargar la cita con el cliente para el DTO
                await _context.Entry(nuevaCita).Reference(c => c.Cliente).LoadAsync();
                var citaDto = _mapper.Map<CitaDto>(nuevaCita);

                return (true, "Cita creada exitosamente", citaDto);
            }
            catch (Exception ex)
            {
                return (false, $"Error al crear cita: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, List<CitaDto> Citas)> ObtenerCitasClienteAsync(long clienteId)
        {
            try
            {
                var citas = await _context.Citas
                    .Include(c => c.Cliente)
                    .Where(c => c.ClienteId == clienteId)
                    .OrderByDescending(c => c.FechaHora)
                    .ToListAsync();

                var citasDto = _mapper.Map<List<CitaDto>>(citas);
                return (true, "Citas obtenidas exitosamente", citasDto);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener citas: {ex.Message}", new List<CitaDto>());
            }
        }

        public async Task<(bool Success, string Message, List<CitaDto> Citas)> ObtenerTodasLasCitasAsync()
        {
            try
            {
                var citas = await _context.Citas
                    .Include(c => c.Cliente)
                    .OrderByDescending(c => c.FechaHora)
                    .ToListAsync();

                var citasDto = _mapper.Map<List<CitaDto>>(citas);
                return (true, "Citas obtenidas exitosamente", citasDto);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener citas: {ex.Message}", new List<CitaDto>());
            }
        }

        public async Task<(bool Success, string Message)> ActualizarEstadoCitaAsync(long citaId, ActualizarEstadoCitaDto actualizarDto)
        {
            try
            {
                var cita = await _context.Citas.FindAsync(citaId);
                if (cita == null)
                {
                    return (false, "Cita no encontrada");
                }

                // Validar estados permitidos
                var estadosPermitidos = new[] { "PENDIENTE", "CONFIRMADA", "RECHAZADA", "COMPLETADA", "CANCELADA" };
                if (!estadosPermitidos.Contains(actualizarDto.Estado))
                {
                    return (false, "Estado inválido");
                }

                cita.Estado = actualizarDto.Estado;
                cita.UpdatedAt = DateTime.UtcNow;

                // Si se confirma la cita, generar código de confirmación
                if (actualizarDto.Estado == "CONFIRMADA" && string.IsNullOrEmpty(cita.CodigoConfirmacion))
                {
                    cita.CodigoConfirmacion = GenerarCodigoConfirmacion();
                }

                // Si se proporciona un código, usarlo
                if (!string.IsNullOrEmpty(actualizarDto.CodigoConfirmacion))
                {
                    cita.CodigoConfirmacion = actualizarDto.CodigoConfirmacion;
                }

                await _context.SaveChangesAsync();
                return (true, "Estado de cita actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return (false, $"Error al actualizar cita: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> EliminarCitaAsync(long citaId)
        {
            try
            {
                var cita = await _context.Citas.FindAsync(citaId);
                if (cita == null)
                {
                    return (false, "Cita no encontrada");
                }

                _context.Citas.Remove(cita);
                await _context.SaveChangesAsync();

                return (true, "Cita eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return (false, $"Error al eliminar cita: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message, CitaDto? Cita)> ObtenerCitaPorIdAsync(long citaId)
        {
            try
            {
                var cita = await _context.Citas
                    .Include(c => c.Cliente)
                    .FirstOrDefaultAsync(c => c.Id == citaId);

                if (cita == null)
                {
                    return (false, "Cita no encontrada", null);
                }

                var citaDto = _mapper.Map<CitaDto>(cita);
                return (true, "Cita encontrada", citaDto);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener cita: {ex.Message}", null);
            }
        }

        private string GenerarCodigoConfirmacion()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}