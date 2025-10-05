namespace barberiaApp.Models
{
    public class ApiResponse<T>
    {
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    public class CitasResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<CitaDto> Citas { get; set; } = new();
    }
}