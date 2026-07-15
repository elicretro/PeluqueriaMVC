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
            var emailUsuario = User.Identity?.Name;
            var clienteLogueado = await _context.Personas
                .OfType<Cliente>()
                .FirstOrDefaultAsync(c => c.Email == emailUsuario);

            if (clienteLogueado != null)
            {
                turno.ClienteId = clienteLogueado.Id;
            }

            turno.Estado = EstadoTurno.Pendiente;

            ModelState.Remove("Estado");
            ModelState.Remove("ClienteId");
        }

        // 1. Validación de compatibilidad de servicio y empleado en el servidor
        var empleado = await _context.Empleados.FindAsync(turno.EmpleadoId);
        if (empleado != null)
        {
            if (!EsEmpleadoAptoParaServicio(empleado.Puesto, turno.Tipo))
            {
                ModelState.AddModelError("EmpleadoId", $"El empleado {empleado.Nombre} {empleado.Apellido} ({empleado.Puesto}) no realiza el servicio de {turno.Tipo}.");
            }
        }
        else
        {
            ModelState.AddModelError("EmpleadoId", "El empleado seleccionado no es válido.");
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

        await CargarDesplegablesConFiltro(turno);
        return View(turno);
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

        await CargarDesplegablesConFiltro(turno);
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

        bool esEmpleado = User.IsInRole("Empleado");
        if (!esEmpleado)
        {
            ModelState.Remove("Estado");
        }

        // Validación de puesto del empleado en edición
        var empleado = await _context.Empleados.FindAsync(turno.EmpleadoId);
        if (empleado != null)
        {
            if (!EsEmpleadoAptoParaServicio(empleado.Puesto, turno.Tipo))
            {
                ModelState.AddModelError("EmpleadoId", $"El empleado {empleado.Nombre} {empleado.Apellido} ({empleado.Puesto}) no realiza el servicio de {turno.Tipo}.");
            }
        }
        else
        {
            ModelState.AddModelError("EmpleadoId", "El empleado seleccionado no es válido.");
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

        await CargarDesplegablesConFiltro(turno);
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

    // =========================================================
    // ENDPOINT AJAX: Retorna los empleados aptos según el TipoServicio elegido
    // =========================================================
    [HttpGet]
    public async Task<IActionResult> GetEmpleadosPorServicio(TipoServicio tipoServicio)
    {
        var todosLosEmpleados = await _context.Empleados.ToListAsync();

        // Filtramos en memoria aplicando la regla de negocio
        var empleadosFiltrados = todosLosEmpleados
            .Where(e => EsEmpleadoAptoParaServicio(e.Puesto, tipoServicio))
            .Select(e => new {
                id = e.Id,
                nombreCompleto = $"{e.Apellido}, {e.Nombre} ({e.Puesto})"
            })
            .ToList();

        return Json(empleadosFiltrados);
    }

    // =========================================================
    // MÉTODOS AUXILIARES PRIVADOS
    // =========================================================

    private bool TurnoExists(int? idturno)
    {
        return _context.Turnos.Any(e => e.IdTurno == idturno);
    }

    // Carga los desplegables aplicando filtros dinámicos iniciales si ya hay un turno con datos
    private async Task CargarDesplegablesConFiltro(Turno turno = null)
    {
        bool esEmpleado = User.IsInRole("Empleado");
        var emailUsuario = User.Identity?.Name;

        if (esEmpleado)
        {
            var clientes = await _context.Personas.OfType<Cliente>().ToListAsync();
            ViewData["ClienteId"] = new SelectList(clientes, "Id", "Apellido", turno?.ClienteId);
            ViewData["MascotaId"] = new SelectList(_context.Mascotas, "Id", "Nombre", turno?.MascotaId);
        }
        else
        {
            var clienteLogueado = await _context.Personas
                .OfType<Cliente>()
                .Include(c => c.Mascotas)
                .FirstOrDefaultAsync(c => c.Email == emailUsuario);

            int clienteId = clienteLogueado?.Id ?? 0;

            ViewData["ClienteId"] = new SelectList(new[] { clienteLogueado }.Where(c => c != null), "Id", "Apellido", clienteId);

            var mascotasDelCliente = clienteLogueado?.Mascotas ?? new List<Mascota>();
            ViewData["MascotaId"] = new SelectList(mascotasDelCliente, "Id", "Nombre", turno?.MascotaId);
        }

        // Si ya hay un tipo de servicio preseleccionado, filtramos los empleados iniciales, si no, traemos a los no-recepcionistas
        List<Empleado> empleadosDisponibles;
        if (turno != null)
        {
            var todos = await _context.Empleados.ToListAsync();
            empleadosDisponibles = todos.Where(e => EsEmpleadoAptoParaServicio(e.Puesto, turno.Tipo)).ToList();
        }
        else
        {
            empleadosDisponibles = await _context.Empleados.Where(e => e.Puesto != TipoPuesto.Recepcionista).ToListAsync();
        }

        ViewData["EmpleadoId"] = new SelectList(empleadosDisponibles, "Id", "Apellido", turno?.EmpleadoId);
        ViewData["TipoServicioList"] = new SelectList(Enum.GetValues(typeof(TipoServicio)), turno?.Tipo);
        ViewData["EstadoTurnoList"] = new SelectList(Enum.GetValues(typeof(EstadoTurno)), turno?.Estado);
    }

    // Regla de compatibilidad centralizada (Para no repetir lógica en Create, Edit y Endpoint AJAX)
    private static bool EsEmpleadoAptoParaServicio(TipoPuesto puesto, TipoServicio servicio)
    {
        // El recepcionista nunca trabaja en servicios estéticos o de salud
        if (puesto == TipoPuesto.Recepcionista) return false;

        // Adaptá los nombres de tus Enums de TipoServicio si se llaman diferente
        switch (servicio)
        {
            case TipoServicio.Peluqueria:
                return puesto == TipoPuesto.Peluquero;

            case TipoServicio.Veterinaria:
                return puesto == TipoPuesto.Veterinario;

            default:
                return false;
        }
    }
}