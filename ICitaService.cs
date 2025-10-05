using barberiaApp.Models;

namespace barberiaApp.Services
{
    public interface ICitaService
    {
        Task<List<CitaDto>> ObtenerMisCitasAsync();
        Task<List<CitaDto>> ObtenerTodasLasCitasAsync();
        Task<bool> CrearCitaAsync(CrearCitaDto citaDto);
        Task<bool> ActualizarEstadoAsync(long citaId, ActualizarEstadoCitaDto estadoDto);
        Task<bool> EliminarCitaAsync(long citaId);
    }
}