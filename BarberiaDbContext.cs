using Microsoft.EntityFrameworkCore;
using BarberappAPI.Models;

namespace BarberappAPI.Data
{
    public class BarberiaDbContext : DbContext
    {
        public BarberiaDbContext(DbContextOptions<BarberiaDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cita> Citas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => new { e.Id }).HasDatabaseName("PRIMARY");
            });

            // Configuración para Cita
            modelBuilder.Entity<Cita>(entity =>
            {
                entity.HasIndex(e => new { e.ClienteId, e.FechaHora }).HasDatabaseName("idx_cliente_fecha");
                entity.HasIndex(e => new { e.Estado, e.FechaHora }).HasDatabaseName("idx_estado_fecha");

                entity.HasOne(d => d.Cliente)
                    .WithMany(p => p.Citas)
                    .HasForeignKey(d => d.ClienteId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}