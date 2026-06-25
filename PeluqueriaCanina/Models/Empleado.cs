using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models
{
    public class Empleado : Persona
    {
        public int Legajo { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Puesto { get; set; }
        public TipoServicio Especialidad { get; set; }
    }
}
