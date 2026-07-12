using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Helpers;
using PeluqueriaCanina.Models;

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
            var carrito = HttpContext.Session
                .GetObjectFromJson<Carrito>("Carrito")
                ?? new Carrito();

            return View(carrito);
        }


        public IActionResult Agregar(int id)
        {
            return RedirectToAction("Index");
        }

        public IActionResult Eliminar(int id)
        {
            return RedirectToAction("Index");
        }

        public IActionResult Vaciar()
        {
            return RedirectToAction("Index");
        }
    }
}