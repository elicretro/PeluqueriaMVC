using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Models;

namespace PeluqueriaCanina.Data
{
    public class PeluqueriaContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<PeluqueriaCanina.Models.Venta> Venta { get; set; } = default!;

        public PeluqueriaContext(DbContextOptions<PeluqueriaContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relaciones de Turno existentes
            modelBuilder.Entity<Turno>().HasOne(t => t.Cliente).WithMany().HasForeignKey(t => t.ClienteId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Turno>().HasOne(t => t.Mascota).WithMany().HasForeignKey(t => t.MascotaId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Turno>().HasOne(t => t.Empleado).WithMany().HasForeignKey(t => t.EmpleadoId).OnDelete(DeleteBehavior.NoAction);

            // Configurar relación entre ApplicationUser y Persona
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(au => au.Persona)
                .WithMany()
                .HasForeignKey(au => au.PersonaId)
                .OnDelete(DeleteBehavior.SetNull);

            // ==========================================
            // SOLUCIÓN AL ERROR DE SQL SERVER (CASCADA MÚLTIPLE):
            // Evitamos que borrar un Cliente intente borrar sus mascotas en cascada.
            // ==========================================
            modelBuilder.Entity<Mascota>()
                .HasOne(m => m.Cliente)
                .WithMany(c => c.Mascotas)
                .HasForeignKey(m => m.ClienteId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<ItemCarrito> ItemsCarrito { get; set; }
        public DbSet<HistoriaClinica> HistoriasClinicas { get; set; }
    }
}