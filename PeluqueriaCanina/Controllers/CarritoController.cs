using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Helpers;
using PeluqueriaCanina.Models;
using System.Linq;

namespace PeluqueriaCanina.Controllers
{
    public class CarritoController : Controller
    {
        private readonly PeluqueriaContext _context;

        public CarritoController(PeluqueriaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var carrito = HttpContext.Session.GetObjectFromJson<Carrito>("Carrito") ?? new Carrito();
            return View(carrito);
        }

        [HttpGet]
        public IActionResult Agregar(int id)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null)
            {
                return NotFound();
            }

            var carrito = HttpContext.Session.GetObjectFromJson<Carrito>("Carrito") ?? new Carrito();

            var itemExistente = carrito.Items.FirstOrDefault(i => i.Producto != null && i.Producto.Id == id);

            if (itemExistente != null)
            {
                itemExistente.Cantidad++;
            }
            else
            {
                carrito.Items.Add(new ItemCarrito
                {
                    Producto = producto,
                    Cantidad = 1
                });
            }

            HttpContext.Session.SetObjectAsJson("Carrito", carrito);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var carrito = HttpContext.Session.GetObjectFromJson<Carrito>("Carrito") ?? new Carrito();

            var item = carrito.Items.FirstOrDefault(i => i.Producto != null && i.Producto.Id == id);
            if (item != null)
            {
                carrito.Items.Remove(item);
            }

            HttpContext.Session.SetObjectAsJson("Carrito", carrito);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Vaciar()
        {
            HttpContext.Session.Remove("Carrito");
            return RedirectToAction("Index");
        }
    }
}