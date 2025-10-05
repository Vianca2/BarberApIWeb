namespace barberiaApp.Models
{
    public class UsuarioDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class RegistroUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Rol { get; set; } = "CLIENTE";
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Message { get; set; } = string.Empty;
        public UsuarioDto? Usuario { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
