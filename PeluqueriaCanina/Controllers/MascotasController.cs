
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Models;
using PeluqueriaCanina.Data;

[Authorize]
public class MascotasController : Controller
{
    private readonly PeluqueriaContext _context;

    public MascotasController(PeluqueriaContext context)
    {
        _context = context;
    }

    // GET: MASCOTAS
    public async Task<IActionResult> Index()
    {
        bool esEmpleado = User.IsInRole("Empleado"); // Reemplazar por tu validación de roles real

        if (esEmpleado)
        {
            // El empleado ve todas las mascotas registradas en el sistema
            return View(await _context.Mascotas.ToListAsync());
        }
        else
        {
            // El cliente solo ve sus propias mascotas
            var emailUsuario = User.Identity?.Name;

            // Buscamos al cliente logueado con sus mascotas cargadas (Eager Loading)
            var clienteLogueado = await _context.Personas
                .OfType<Cliente>()
                .Include(c => c.Mascotas)
                .FirstOrDefaultAsync(c => c.Email == emailUsuario);

            var misMascotas = clienteLogueado?.Mascotas ?? new List<Mascota>();
            return View(misMascotas);
        }
    }

    // GET: MASCOTAS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var mascota = await _context.Mascotas
            .FirstOrDefaultAsync(m => m.Id == id);
        if (mascota == null)
        {
            return NotFound();
        }

        return View(mascota);
    }

    // GET: MASCOTAS/Create
    public async Task<IActionResult> Create(int? idCliente)
    {
        bool esEmpleado = User.IsInRole("Empleado");

        if (!esEmpleado)
        {
            // Si es cliente, autodetectamos su ID para que no tenga que pasarse por parámetro
            var emailUsuario = User.Identity?.Name;
            var clienteLogueado = await _context.Personas
                .OfType<Cliente>()
                .FirstOrDefaultAsync(c => c.Email == emailUsuario);

            if (clienteLogueado != null)
            {
                ViewBag.IdCliente = clienteLogueado.Id;
            }
        }
        else
        {
            // El empleado sí usa el idCliente recibido desde el perfil del cliente
            ViewBag.IdCliente = idCliente;
        }

        return View();
    }

    // POST: MASCOTAS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Edad,Tipo,Raza,Peso")] Mascota mascota, int idCliente)
    {
        // Limpiamos errores de validación de campos que no enviamos
        ModelState.Remove("Identificacion");
        ModelState.Remove("Cliente");

        bool esEmpleado = User.IsInRole("Empleado");

        // Si es un cliente, por seguridad volvemos a verificar su ID de base de datos
        if (!esEmpleado)
        {
            var emailUsuario = User.Identity?.Name;
            var clienteLogueado = await _context.Personas
                .OfType<Cliente>()
                .FirstOrDefaultAsync(c => c.Email == emailUsuario);

            if (clienteLogueado != null)
            {
                idCliente = clienteLogueado.Id;
            }
        }

        if (ModelState.IsValid)
        {
            var cliente = await _context.Personas.FindAsync(idCliente) as Cliente;
            if (cliente != null)
            {
                if (cliente.Mascotas == null)
                {
                    cliente.Mascotas = new List<Mascota>();
                }

                cliente.Mascotas.Add(mascota);
                await _context.SaveChangesAsync();

                // Redirección inteligente según quién sea:
                if (esEmpleado)
                {
                    // Al empleado lo mandamos de vuelta al perfil de ese cliente
                    return RedirectToAction("Edit", "Personas", new { id = idCliente });
                }
                else
                {
                    // Al cliente lo mandamos a su lista general de mascotas
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // Si falla algo, recargamos el ViewBag para no perder el ID
        ViewBag.IdCliente = idCliente;
        return View(mascota);
    }

    // GET: MASCOTAS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var mascota = await _context.Mascotas.FindAsync(id);
        if (mascota == null)
        {
            return NotFound();
        }
        return View(mascota);
    }

    // POST: MASCOTAS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Edad,Tipo,Raza,Peso")] Mascota mascota, int idCliente)
    {
        if (id != mascota.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(mascota);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MascotaExists(mascota.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            bool esEmpleado = User.IsInRole("Empleado");
            if (esEmpleado)
            {
                return RedirectToAction("Details", "Personas", new { id = idCliente });
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return View(mascota);
    }

    // GET: MASCOTAS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var mascota = await _context.Mascotas
            .FirstOrDefaultAsync(m => m.Id == id);
        if (mascota == null)
        {
            return NotFound();
        }

        return View(mascota);
    }

    // POST: MASCOTAS/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMascotaPost(int id, int idCliente)
    {
        var mascota = await _context.Mascotas.FindAsync(id);
        if (mascota != null)
        {
            _context.Mascotas.Remove(mascota);
            await _context.SaveChangesAsync();
        }

        bool esEmpleado = User.IsInRole("Empleado");
        if (esEmpleado)
        {
            return RedirectToAction("Details", "Personas", new { id = idCliente });
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    private bool MascotaExists(int? id)
    {
        return _context.Mascotas.Any(e => e.Id == id);
    }
}