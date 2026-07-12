using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [MinLength(3, ErrorMessage = ErrMsgs.StrMin)]
        [MaxLength(100, ErrorMessage = ErrMsgs.StrMax)]
        [Display(Name = Alias.NombreProducto)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [MinLength(10, ErrorMessage = ErrMsgs.StrMin)]
        [MaxLength(500, ErrorMessage = ErrMsgs.StrMax)]
        [Display(Name = Alias.DescripcionProducto)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = ErrMsgs.Requerido)]
        [Range(1, 1000000, ErrorMessage = ErrMsgs.Range)]
        [Display(Name = Alias.PrecioProducto)]
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public CategoriasProductos Categoria { get; set; }
        public string PresentacionComercial => $"{Nombre} (${Precio}) - Stock: {Stock}";

        public int CategoriaId { get; set; }

  

    }
}
