using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models
{
    public class Mascota
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [RegularExpression(@"^[a-zA-Z áéíóúÁÉÍÓÚñÑ]*$", ErrorMessage = ErrMsgs.StrSoloAlfab)]
        [MinLength(2, ErrorMessage = ErrMsgs.StrMin)]
        [MaxLength(50, ErrorMessage = ErrMsgs.StrMax)]
        [Display(Name = Alias.NombreMascota)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [Range(0, 30, ErrorMessage = ErrMsgs.Range)]
        [Display(Name = Alias.EdadMascota)]
        public int Edad { get; set; }

        public TipoMascota Tipo { get; set; }
        public string Raza { get; set; }
        public string Identificacion => $"{Nombre} ({Tipo})";

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [Range(0.1, 150.0, ErrorMessage = ErrMsgs.Range)]
        [Display(Name = Alias.PesoMascota)]
        public double Peso { get; set; }
    }
}
