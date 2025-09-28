using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberappAPI.Models
{
    [Table("citas")]
    public class Cita
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [Column("cliente_id")]
        public long ClienteId { get; set; }

        [Required]
        [Column("fecha_hora")]
        public DateTime FechaHora { get; set; }

        [Required]
        [StringLength(100)]
        [Column("tipo_corte")]
        public string TipoCorte { get; set; }

        [Column(TypeName = "TEXT")]
        public string? Descripcion { get; set; }

        [Column(TypeName = "enum('PENDIENTE','CONFIRMADA','RECHAZADA','COMPLETADA','CANCELADA')")]
        public string Estado { get; set; } = "PENDIENTE";

        [StringLength(10)]
        [Column("codigo_confirmacion")]
        public string? CodigoConfirmacion { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ClienteId")]
        public virtual Usuario Cliente { get; set; }
    }
}