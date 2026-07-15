
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Models;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Helpers;
using Microsoft.AspNetCore.Identity;
public class VentasController : Controller
{
    private readonly PeluqueriaContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
 
    public VentasController(PeluqueriaContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: VENTAS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Ventas.ToListAsync());
    }

    // GET: VENTAS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var venta = await _context.Ventas
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

        var venta = await _context.Ventas.FindAsync(id);
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

        var venta = await _context.Ventas
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
        var venta = await _context.Ventas.FindAsync(id);
        if (venta != null)
        {
            _context.Ventas.Remove(venta);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Checkout()
    {
        var carrito = HttpContext.Session
            .GetObjectFromJson<Carrito>("Carrito")
            ?? new Carrito();

        return View(carrito);
    }

    private bool VentaExists(int? id)
    {
        return _context.Ventas.Any(e => e.Id == id);
    }
    [HttpPost]
    public async Task<IActionResult> ConfirmarCompra(MetodoDePago metodoPago)
    {
        var carrito = HttpContext.Session
            .GetObjectFromJson<Carrito>("Carrito")
            ?? new Carrito();

        var usuario = await _userManager.GetUserAsync(User);

        if (usuario == null)
        {
            return Challenge(); // o RedirectToAction("Login", "Account")
        }

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == usuario.PersonaId);

        if (cliente == null)
        {
            return Content("No se encontró el cliente.");
        }

        var venta = new Venta
        {
            Fecha = DateTime.Now,
            Cliente = cliente,
            MetodoPago = metodoPago,
            Total = carrito.Total,
            Detalle = carrito.Items
        };

        _context.Ventas.Add(venta);

        await _context.SaveChangesAsync();

        HttpContext.Session.Remove("Carrito");

        return RedirectToAction(nameof(Index));
    }
}
