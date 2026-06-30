using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeluqueriaCanina.Models
{
    public class Turno
    {
        [Key]
        public int IdTurno { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        [Display(Name = "Horario del Turno")]
        public DateTime FechaHora { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        public int MascotaId { get; set; }
        public Mascota Mascota { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }

        public EstadoTurno Estado { get; set; } = EstadoTurno.Pendiente;
        public TipoServicio Tipo { get; set; }

        public string ResumenCita => $"[{Tipo}] {FechaHora:dd/MM HH:mm} - {Mascota?.Nombre} ({Cliente?.Apellido})";
    }
}
