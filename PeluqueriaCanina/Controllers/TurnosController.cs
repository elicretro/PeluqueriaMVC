
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Models;
using PeluqueriaCanina.Data;

public class TurnosController : Controller
{
    private readonly PeluqueriaContext _context;

    public TurnosController(PeluqueriaContext context)
    {
        _context = context;
    }

    // GET: Turnos
    public async Task<IActionResult> Index()
    {
        // Agregamos .Include para traer los datos reales de los clientes, mascotas y empleados
        var turnos = await _context.Turnos
            .Include(t => t.Cliente)
            .Include(t => t.Mascota)
            .Include(t => t.Empleado)
            .ToListAsync();

        return View(turnos);
    }

    // GET: Turnos/Details/5
    public async Task<IActionResult> Details(int? idturno)
    {
        if (idturno == null)
        {
            return NotFound();
        }

        var turno = await _context.Turnos
            .Include(t => t.Cliente)
            .Include(t => t.Mascota)
            .Include(t => t.Empleado)
            .FirstOrDefaultAsync(m => m.IdTurno == idturno);

        if (turno == null)
        {
            return NotFound();
        }

        return View(turno);
    }

    // GET: Turnos/Create
    public async Task<IActionResult> Create()
    {
        await CargarDesplegablesConFiltro();
        return View();
    }

    // POST: Turnos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdTurno,FechaHora,ClienteId,MascotaId,EmpleadoId,Estado,Tipo")] Turno turno)
    {
        bool esEmpleado = User.IsInRole("Empleado");

        if (!esEmpleado)
        {
            // 1. Buscamos el cliente logueado por su email (User.Identity.Name)
            var emailUsuario = User.Identity?.Name;
            var clienteLogueado = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == emailUsuario);

            if (clienteLogueado != null)
            {
                // Asignamos automáticamente el ID del cliente logueado
                turno.ClienteId = clienteLogueado.Id;
            }

            turno.Estado = EstadoTurno.Pendiente;

            ModelState.Remove("Estado");
            ModelState.Remove("ClienteId"); // Removemos de la validación porque lo asignamos acá
        }

        // 2. Validación de fecha pasada
        if (turno.FechaHora < DateTime.Now)
        {
            ModelState.AddModelError("FechaHora", "No se pueden programar turnos para fechas pasadas.");
        }

        // 3. Validación de rango de atención (08:00 a 19:00 hs)
        int hora = turno.FechaHora.Hour;
        if (hora < 8 || hora >= 19)
        {
            ModelState.AddModelError("FechaHora", "El horario del turno debe estar entre las 08:00 y las 19:00 hs.");
        }

        // 4. Evitar solapamiento de horarios (Duración estimada: 1 hora)
        if (ModelState.IsValid)
        {
            DateTime inicioNuevo = turno.FechaHora;
            DateTime finNuevo = turno.FechaHora.AddHours(1);

            bool empleadoOcupado = await _context.Turnos.AnyAsync(t =>
                t.EmpleadoId == turno.EmpleadoId &&
                t.FechaHora < finNuevo &&
                t.FechaHora.AddHours(1) > inicioNuevo
            );

            if (empleadoOcupado)
            {
                ModelState.AddModelError("FechaHora", "El empleado seleccionado ya tiene un turno asignado que se superpone con este horario.");
            }

            bool mascotaOcupada = await _context.Turnos.AnyAsync(t =>
                t.MascotaId == turno.MascotaId &&
                t.FechaHora < finNuevo &&
                t.FechaHora.AddHours(1) > inicioNuevo
            );

            if (mascotaOcupada)
            {
                ModelState.AddModelError("MascotaId", "Esta mascota ya tiene otro turno programado en un horario que se superpone.");
            }
        }

        if (ModelState.IsValid)
        {
            _context.Add(turno);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Si falló, volvemos a cargar filtrando según corresponda
        await CargarDesplegablesConFiltro(turno);
        return View(turno);
    }

    // NUEVO Método auxiliar asincrónico para cargar desplegables filtrados por usuario
    private async Task CargarDesplegablesConFiltro(Turno turno = null)
    {
        bool esEmpleado = User.IsInRole("Empleado");
        var emailUsuario = User.Identity?.Name;

        if (esEmpleado)
        {
            // Traemos los clientes usando OfType<Cliente>()
            var clientes = await _context.Personas.OfType<Cliente>().ToListAsync();
            ViewData["ClienteId"] = new SelectList(clientes, "Id", "Apellido", turno?.ClienteId);
            ViewData["MascotaId"] = new SelectList(_context.Mascotas, "Id", "Nombre", turno?.MascotaId);
        }
        else
        {
            // Buscamos el ID del cliente logueado desde la tabla Personas filtrando el tipo
            var clienteLogueado = await _context.Personas
                .OfType<Cliente>()
                .Include(c => c.Mascotas)
                .FirstOrDefaultAsync(c => c.Email == emailUsuario);

            int clienteId = clienteLogueado?.Id ?? 0;

            ViewData["ClienteId"] = new SelectList(new[] { clienteLogueado }.Where(c => c != null), "Id", "Apellido", clienteId);

            var mascotasDelCliente = clienteLogueado?.Mascotas ?? new List<Mascota>();
            ViewData["MascotaId"] = new SelectList(mascotasDelCliente, "Id", "Nombre", turno?.MascotaId);
        }

        ViewData["EmpleadoId"] = new SelectList(_context.Empleados, "Id", "Apellido", turno?.EmpleadoId);
        ViewData["TipoServicioList"] = new SelectList(Enum.GetValues(typeof(TipoServicio)));
        ViewData["EstadoTurnoList"] = new SelectList(Enum.GetValues(typeof(EstadoTurno)));
    }

    // GET: Turnos/Edit/5
    public async Task<IActionResult> Edit(int? idturno)
    {
        if (idturno == null)
        {
            return NotFound();
        }

        var turno = await _context.Turnos.FindAsync(idturno);
        if (turno == null)
        {
            return NotFound();
        }

        CargarDesplegables(turno);
        return View(turno);
    }

    // POST: Turnos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? idturno, [Bind("IdTurno,FechaHora,ClienteId,MascotaId,EmpleadoId,Estado,Tipo")] Turno turno)
    {
        if (idturno != turno.IdTurno)
        {
            return NotFound();
        }

        // Al igual que en la creación, si el usuario no es empleado no debería poder alterar el Estado
        bool esEmpleado = User.IsInRole("Empleado");
        if (!esEmpleado)
        {
            ModelState.Remove("Estado");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(turno);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TurnoExists(turno.IdTurno))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        CargarDesplegables(turno);
        return View(turno);
    }

    // GET: Turnos/Delete/5
    public async Task<IActionResult> Delete(int? idturno)
    {
        if (idturno == null)
        {
            return NotFound();
        }

        var turno = await _context.Turnos
            .Include(t => t.Cliente)
            .Include(t => t.Mascota)
            .Include(t => t.Empleado)
            .FirstOrDefaultAsync(m => m.IdTurno == idturno);

        if (turno == null)
        {
            return NotFound();
        }

        return View(turno);
    }

    // POST: Turnos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? idturno)
    {
        var turno = await _context.Turnos.FindAsync(idturno);
        if (turno != null)
        {
            _context.Turnos.Remove(turno);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TurnoExists(int? idturno)
    {
        return _context.Turnos.Any(e => e.IdTurno == idturno);
    }

    // Método auxiliar para evitar repetir código de los SelectList
    private void CargarDesplegables(Turno turno = null)
    {
        ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", turno?.ClienteId);
        ViewData["MascotaId"] = new SelectList(_context.Mascotas, "Id", "Nombre", turno?.MascotaId);
        ViewData["EmpleadoId"] = new SelectList(_context.Empleados, "Id", "Apellido", turno?.EmpleadoId);

        // Cargamos los Enums para pasárselos a la vista como listas
        ViewData["TipoServicioList"] = new SelectList(Enum.GetValues(typeof(TipoServicio)));
        ViewData["EstadoTurnoList"] = new SelectList(Enum.GetValues(typeof(EstadoTurno)));
    }
}