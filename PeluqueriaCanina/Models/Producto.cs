using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public CategoriasProductos Categoria { get; set; }
        public string PresentacionComercial => $"{Nombre} (${Precio}) - Stock: {Stock}";
    }
}
