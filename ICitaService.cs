using BarberappAPI.DTOs;

namespace BarberappAPI.Services
{
    public interface ICitaService
    {
        Task<(bool Success, string Message, CitaDto? Cita)> CrearCitaAsync(long clienteId, CrearCitaDto crearCitaDto);
        Task<(bool Success, string Message, List<CitaDto> Citas)> ObtenerCitasClienteAsync(long clienteId);
        Task<(bool Success, string Message, List<CitaDto> Citas)> ObtenerTodasLasCitasAsync();
        Task<(bool Success, string Message)> ActualizarEstadoCitaAsync(long citaId, ActualizarEstadoCitaDto actualizarDto);
        Task<(bool Success, string Message)> EliminarCitaAsync(long citaId);
        Task<(bool Success, string Message, CitaDto? Cita)> ObtenerCitaPorIdAsync(long citaId);
    }
}