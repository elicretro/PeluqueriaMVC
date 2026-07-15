using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Helpers;
using PeluqueriaCanina.Models;
using PeluqueriaCanina.Data;
namespace PeluqueriaCanina.Controllers
{
    public class CarritosController : Controller
    {
        public IActionResult Index()
        {
            var carrito = HttpContext.Session
                .GetObjectFromJson<Carrito>("Carrito")
                ?? new Carrito();

            return View(carrito);
        }

        private readonly PeluqueriaContext _context;

        public CarritosController(PeluqueriaContext context)
        {
            _context = context;
        }
        public IActionResult Agregar(int id)
        {
            var producto = _context.Productos
                .FirstOrDefault(p => p.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            var carrito = HttpContext.Session
                .GetObjectFromJson<Carrito>("Carrito")
                ?? new Carrito();

            var itemExistente = carrito.Items
                .FirstOrDefault(i => i.Producto.Id == id);

            if (itemExistente != null)
            {
                itemExistente.Cantidad++;
            }
            else
            {
                carrito.Items.Add(new ItemCarrito
                {

                    Producto = producto,
                    ProductoId = producto.Id,
                    Cantidad = 1

                });
            }

            HttpContext.Session.SetObjectAsJson(
                "Carrito",
                carrito);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Eliminar(int id)
        {
            var carrito = HttpContext.Session
                .GetObjectFromJson<Carrito>("Carrito")
                ?? new Carrito();

            var item = carrito.Items
                .FirstOrDefault(i => i.Producto.Id == id);

            if (item != null)
            {
                carrito.Items.Remove(item);
            }

            HttpContext.Session.SetObjectAsJson(
                "Carrito",
                carrito);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Vaciar()
        {
            HttpContext.Session.Remove("Carrito");

            return RedirectToAction(nameof(Index));
        }




    }
}