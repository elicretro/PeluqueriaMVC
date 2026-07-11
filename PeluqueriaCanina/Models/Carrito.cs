namespace PeluqueriaCanina.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Carrito
    {
        public Cliente Cliente { get; set; }
        public List<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();
        public decimal Total
        {
            get
            {
                return Items.Sum(item => item.Subtotal);
            }
        }
    }
}
