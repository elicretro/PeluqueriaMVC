using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models
{
    public class Empleado : Persona
    {
        public int Legajo { get; set; }
        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [RegularExpression(@"^[a-zA-Z áéíóúÁÉÍÓÚñÑ]*$", ErrorMessage = ErrMsgs.StrSoloAlfab)]
        [MinLength(3, ErrorMessage = ErrMsgs.StrMin)]
        [MaxLength(50, ErrorMessage = ErrMsgs.StrMax)]
        [Display(Name = Alias.PuestoEmpleado)]
        public string Puesto { get; set; }
        public TipoServicio Especialidad { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [Range(1, 5000000, ErrorMessage = ErrMsgs.Range)]
        [Display(Name = Alias.SueldoEmpleado)]
        public decimal Sueldo { get; set; }
    }
}
