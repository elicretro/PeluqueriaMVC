using System.ComponentModel.DataAnnotations.Schema;

namespace PeluqueriaCanina.Models
{
    public class Turno
    {
        public int IdTurno { get; set; }
        public DateTime FechaHora { get; set; }
        public Cliente Cliente { get; set; }
        public Mascota Mascota { get; set; }

        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }

        public EstadoTurno Estado { get; set; } = EstadoTurno.Pendiente;
        public TipoServicio Tipo { get; set; }
        public string ResumenCita => $"[{Tipo}] {FechaHora:dd/MM HH:mm} - {Mascota.Nombre} ({Cliente.Apellido})";
    }
}
