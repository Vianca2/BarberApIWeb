namespace barberiaApp.Models
{
    public class CitaDto
    {
        public long Id { get; set; }
        public long ClienteId { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public string TelefonoCliente { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public string TipoCorte { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? CodigoConfirmacion { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CrearCitaDto
    {
        public DateTime FechaHora { get; set; }
        public string TipoCorte { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class ActualizarEstadoCitaDto
    {
        public string Estado { get; set; } = string.Empty;
        public string? CodigoConfirmacion { get; set; }
    }
}