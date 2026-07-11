using Microsoft.AspNetCore.Identity;

namespace PeluqueriaCanina.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? PersonaId { get; set; }
        public string? TipoUsuario { get; set; } // "Cliente" o "Empleado"
        public bool IsActive { get; set; } = true;

        public virtual Persona? Persona { get; set; }
    }
}
