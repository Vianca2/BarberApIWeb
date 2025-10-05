using barberiaApp.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace barberiaApp.Services
{
    public class CitaService : ICitaService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public CitaService(HttpClient httpClient, IAuthService authService, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _authService = authService;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7139/api");
        }

        private async Task SetAuthHeaderAsync()
        {
            var token = await _authService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<CitaDto>> ObtenerMisCitasAsync()
        {
            try
            {
                await SetAuthHeaderAsync();
                var response = await _httpClient.GetFromJsonAsync<CitasResponse>("/citas/mis-citas");
                return response?.Citas ?? new List<CitaDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener mis citas: {ex.Message}");
                return new List<CitaDto>();
            }
        }

        public async Task<List<CitaDto>> ObtenerTodasLasCitasAsync()
        {
            try
            {
                await SetAuthHeaderAsync();
                var response = await _httpClient.GetFromJsonAsync<CitasResponse>("/citas");
                return response?.Citas ?? new List<CitaDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener todas las citas: {ex.Message}");
                return new List<CitaDto>();
            }
        }

        public async Task<bool> CrearCitaAsync(CrearCitaDto citaDto)
        {
            try
            {
                await SetAuthHeaderAsync();
                var response = await _httpClient.PostAsJsonAsync("/citas", citaDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear cita: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarEstadoAsync(long citaId, ActualizarEstadoCitaDto estadoDto)
        {
            try
            {
                await SetAuthHeaderAsync();
                var response = await _httpClient.PutAsJsonAsync($"/citas/{citaId}/estado", estadoDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar estado: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EliminarCitaAsync(long citaId)
        {
            try
            {
                await SetAuthHeaderAsync();
                var response = await _httpClient.DeleteAsync($"/citas/{citaId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar cita: {ex.Message}");
                return false;
            }
        }
    }
}