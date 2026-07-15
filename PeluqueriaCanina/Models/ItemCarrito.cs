using System.ComponentModel.DataAnnotations.Schema;

namespace PeluqueriaCanina.Models
{
    public class ItemCarrito
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal
        {
            get
            {
                if (Producto == null) return 0;
                return Producto.Precio * Cantidad;
            }
        }
    }
}
