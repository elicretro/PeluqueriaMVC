using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PeluqueriaCanina.Models
{
    public class Persona
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Telefono { get; set; }

        public string NombreCompleto
        {
            get {
                if (string.IsNullOrEmpty(Apellido) && string.IsNullOrEmpty(Nombre)) return "Sin Definir";
                if (string.IsNullOrEmpty(Apellido)) return Nombre;
                if (string.IsNullOrEmpty(Nombre)) return Apellido.ToUpper();

                return $"{Apellido.ToUpper()}, {Nombre}";
            }
        }
    }
}
