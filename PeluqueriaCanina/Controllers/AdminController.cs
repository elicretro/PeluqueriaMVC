using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models;

namespace PeluqueriaCanina.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PeluqueriaContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            PeluqueriaContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Admin/Empleados
        public async Task<IActionResult> Empleados()
        {
            var empleados = await _userManager.Users
                .Where(u => u.TipoUsuario == "Empleado")
                .ToListAsync();

            return View(empleados);
        }

        // POST: Admin/ActivarEmpleado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivarEmpleado(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.TipoUsuario != "Empleado")
            {
                return NotFound();
            }

            try
            {
                user.IsActive = true;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"El empleado {user.Email} ha sido activado correctamente.";
                    return RedirectToAction(nameof(Empleados));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al activar el empleado.";
                    return RedirectToAction(nameof(Empleados));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Empleados));
            }
        }

        // POST: Admin/DesactivarEmpleado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesactivarEmpleado(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.TipoUsuario != "Empleado")
            {
                return NotFound();
            }

            try
            {
                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"El empleado {user.Email} ha sido desactivado correctamente.";
                    return RedirectToAction(nameof(Empleados));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al desactivar el empleado.";
                    return RedirectToAction(nameof(Empleados));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Empleados));
            }
        }

        // GET: Admin/VerificacionEmpleado/5
        public async Task<IActionResult> VerificacionEmpleado(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.TipoUsuario != "Empleado")
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .Include(e => e.Especialidad)
                .FirstOrDefaultAsync(e => e.Id == user.PersonaId);

            var viewModel = new
            {
                user,
                empleado
            };

            return View(viewModel);
        }
    }
}
