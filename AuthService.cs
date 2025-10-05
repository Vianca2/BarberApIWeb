using barberiaApp.Models;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace barberiaApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly IConfiguration _configuration;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7139/api");
        }

        public async Task<LoginResponse?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/auth/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        await _localStorage.SetItemAsync("authToken", loginResponse.Token);
                        await _localStorage.SetItemAsync("usuario", loginResponse.Usuario);
                        _httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", loginResponse.Token);
                    }

                    return loginResponse;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en login: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> RegistroAsync(RegistroUsuarioDto registroDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/auth/registro", registroDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en registro: {ex.Message}");
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("usuario");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }
    }
}