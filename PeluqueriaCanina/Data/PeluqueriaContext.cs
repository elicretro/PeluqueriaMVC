using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Models;

namespace PeluqueriaCanina.Data
{
    public class PeluqueriaContext : IdentityDbContext<ApplicationUser>
    {
        public PeluqueriaContext(DbContextOptions<PeluqueriaContext> options) : base(options) {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Turno>().HasOne(t => t.Cliente).WithMany().HasForeignKey(t => t.ClienteId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Turno>().HasOne(t => t.Mascota).WithMany().HasForeignKey(t => t.MascotaId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Turno>().HasOne(t => t.Empleado).WithMany().HasForeignKey(t => t.EmpleadoId).OnDelete(DeleteBehavior.NoAction);

            // Configurar relación entre ApplicationUser y Persona
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(au => au.Persona)
                .WithMany()
                .HasForeignKey(au => au.PersonaId)
                .OnDelete(DeleteBehavior.SetNull);
        }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<HistoriaClinica> HistoriasClinicas { get; set; }


    }
}
