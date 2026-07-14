using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models
{
    public class Empleado : Persona
    {
        public int Legajo { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [Display(Name = "Puesto de Trabajo")]
        public TipoPuesto Puesto { get; set; }
        public TipoServicio Especialidad { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [Range(1, 5000000, ErrorMessage = ErrMsgs.Range)]
        [Display(Name = Alias.SueldoEmpleado)]
        public decimal Sueldo { get; set; }
    }
}
