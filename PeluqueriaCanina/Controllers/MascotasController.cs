
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Models;
using PeluqueriaCanina.Data;

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
        return View(await _context.Mascotas.ToListAsync());
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
    public IActionResult Create()
    {
        return View(); // Esto le dice a ASP.NET: "buscá el archivo llamado 'Create.cshtml' en la carpeta '/Views/Mascotas/'"
    }

    // POST: MASCOTAS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Edad,Tipo,Raza,Peso")] Mascota mascota, int idCliente)
    {
        if (ModelState.IsValid)
        {
            // Buscamos al cliente y agregamos la mascota
            var cliente = await _context.Personas.FindAsync(idCliente) as Cliente;
            if (cliente != null)
            {
                cliente.Mascotas.Add(mascota);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Personas", new { id = idCliente });
            }
        }
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
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Nombre,Edad,Tipo,Raza,Identificacion,Peso")] Mascota mascota)
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
            return RedirectToAction(nameof(Index));
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
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var mascota = await _context.Mascotas.FindAsync(id);
        if (mascota != null)
        {
            _context.Mascotas.Remove(mascota);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MascotaExists(int? id)
    {
        return _context.Mascotas.Any(e => e.Id == id);
    }
}
