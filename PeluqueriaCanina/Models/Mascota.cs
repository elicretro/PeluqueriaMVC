using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models
{
    public class Mascota
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }

        public TipoMascota Tipo { get; set; }
        public string Raza { get; set; }
        public string Identificacion => $"{Nombre} ({Tipo})";
    }
}
