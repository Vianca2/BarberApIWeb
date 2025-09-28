using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberappAPI.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Column(TypeName = "enum('CLIENTE','BARBERO')")]
        public string Rol { get; set; } = "CLIENTE";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Cita> Citas { get; set; }
    }
}