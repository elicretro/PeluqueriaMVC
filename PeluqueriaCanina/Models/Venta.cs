namespace PeluqueriaCanina.Models
{
using System;
using System.Collections.Generic;

public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public Cliente Cliente { get; set; }

        // Copiamos los ítems que estaban en el carrito en ese momento
        public List<ItemCarrito> Detalle { get; set; } = new List<ItemCarrito>();

        public decimal Total { get; set; }
        public MetodoDePago MetodoPago { get; set; } // Ej: "Tarjeta", "Efectivo", "MercadoPago"
    }
}
