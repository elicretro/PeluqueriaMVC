using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Data;

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
            return View();
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