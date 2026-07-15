
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Helpers;
using PeluqueriaCanina.Models;

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
        // return View(await _context.Ventas.ToListAsync());


        return View(
                await _context.Ventas
                    .Include(v => v.Cliente)
                    .Include(v => v.Detalle)
                    .ToListAsync());


    }

    // GET: VENTAS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }


        var venta = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Detalle)
                .ThenInclude(d => d.Producto)
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
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> MisCompras()
    {
        var usuario = await _userManager.GetUserAsync(User);

        if (usuario == null)
        {
            return Challenge();
        }

        var ventas = await _context.Ventas
            .Include(v => v.Cliente)
            .Where(v => v.Cliente.Id == usuario.PersonaId)
            .ToListAsync();

        return View(ventas);
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

        /*  var venta = new Venta
          {
              Fecha = DateTime.Now,
              Cliente = cliente,
              MetodoPago = metodoPago,
              Total = carrito.Total,
              Detalle = carrito.Items
          };*/
        // 1. Verificar stock de todos los productos
        foreach (var item in carrito.Items)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == item.Producto.Id);

            if (producto == null)
            {
                return NotFound($"No existe el producto con Id {item.Producto.Id}.");
            }

            if (producto.Stock < item.Cantidad)
            {
                TempData["Error"] =
      $"No hay stock suficiente para {producto.Nombre}.";

                return RedirectToAction(nameof(Checkout));
            }
        }

        // 2. Crear la venta
        var venta = new Venta
        {
            Fecha = DateTime.Now,
            Cliente = cliente,
            MetodoPago = metodoPago,
            Total = carrito.Total
        };

        _context.Ventas.Add(venta);

        await _context.SaveChangesAsync();

        // 3. Descontar stock y guardar detalle
        foreach (var item in carrito.Items)
        {
            var producto = await _context.Productos
                .FirstAsync(p => p.Id == item.Producto.Id);

            producto.Stock -= item.Cantidad;

            _context.ItemsCarrito.Add(new ItemCarrito
            {
                VentaId = venta.Id,
                ProductoId = producto.Id,
                Cantidad = item.Cantidad
            });
        }

        await _context.SaveChangesAsync();

        HttpContext.Session.Remove("Carrito");

        return RedirectToAction(nameof(Index));
    }
}
