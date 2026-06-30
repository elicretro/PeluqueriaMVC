using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PeluqueriaCanina.Models
{
    public class Persona
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [RegularExpression(@"^[a-zA-Z áéíóúÁÉÍÓÚñÑ]*$", ErrorMessage = ErrMsgs.StrSoloAlfab)]
        [MinLength(2, ErrorMessage = ErrMsgs.StrMin)]
        [MaxLength(100, ErrorMessage = ErrMsgs.StrMax)]
        [Display(Name = Alias.NombreCompleto)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [RegularExpression(@"^[a-zA-Z áéíóúÁÉÍÓÚñÑ]*$", ErrorMessage = ErrMsgs.StrSoloAlfab)]
        [MinLength(2, ErrorMessage = ErrMsgs.StrMin)]
        [MaxLength(100, ErrorMessage = ErrMsgs.StrMax)]
        [Display(Name = Alias.ApellidoCompleto)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [EmailAddress(ErrorMessage = ErrMsgs.NotValid)]
        [MinLength(5, ErrorMessage = ErrMsgs.StrMin)]
        [MaxLength(256, ErrorMessage = ErrMsgs.StrMax)]
        [Display(Name = Alias.CorreoElectronico)]
        public string Email { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "El campo {0} debe contener solo números.")]
        [MinLength(7, ErrorMessage = ErrMsgs.StrMin)]
        [MaxLength(8, ErrorMessage = ErrMsgs.StrMax)]
        [Display(Name = Alias.PersonaDocumento)]
        public string Dni { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [MinLength(8, ErrorMessage = ErrMsgs.StrMin)]
        [MaxLength(20, ErrorMessage = ErrMsgs.StrMax)]
        [Display(Name = Alias.TelefonoContacto)]
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
