using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeluqueriaCanina.Models
{
    public class HistoriaClinica
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        // A qué mascota pertenece este registro
        public Mascota Mascota { get; set; }

        [ForeignKey("VeterionarioId")]
        public Empleado Veterinario { get; set; }

        // Detalles médicos
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Diagnostico { get; set; }
       
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string TratamientoRecetado { get; set; }
        public decimal PesoMascota { get; set; }
    }
}
