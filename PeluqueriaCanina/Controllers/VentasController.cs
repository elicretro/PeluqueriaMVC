
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Models;
using PeluqueriaCanina.Data;

public class VentasController : Controller
{
    private readonly PeluqueriaContext _context;

    public VentasController(PeluqueriaContext context)
    {
        _context = context;
    }

    // GET: VENTAS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Venta.ToListAsync());
    }

    // GET: VENTAS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var venta = await _context.Venta
            .FirstOrDefaultAsync(m => m.Id == id);
        if (venta == null)
        {
            return NotFound();
        }

        return View(venta);
    }

    // GET: VENTAS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: VENTAS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Fecha,Cliente,Detalle,Total,MetodoPago")] Venta venta)
    {
        if (ModelState.IsValid)
        {
            _context.Add(venta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(venta);
    }

    // GET: VENTAS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var venta = await _context.Venta.FindAsync(id);
        if (venta == null)
        {
            return NotFound();
        }
        return View(venta);
    }

    // POST: VENTAS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Fecha,Cliente,Detalle,Total,MetodoPago")] Venta venta)
    {
        if (id != venta.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(venta);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VentaExists(venta.Id))
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
        return View(venta);
    }

    // GET: VENTAS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var venta = await _context.Venta
            .FirstOrDefaultAsync(m => m.Id == id);
        if (venta == null)
        {
            return NotFound();
        }

        return View(venta);
    }

    // POST: VENTAS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var venta = await _context.Venta.FindAsync(id);
        if (venta != null)
        {
            _context.Venta.Remove(venta);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool VentaExists(int? id)
    {
        return _context.Venta.Any(e => e.Id == id);
    }
}
