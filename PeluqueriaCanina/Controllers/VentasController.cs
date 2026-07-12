using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Models;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Helpers;
using System.Linq;
using System.Threading.Tasks;
using System;

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

    // GET: VENTAS/Checkout
    [HttpGet]
    public IActionResult Checkout()
    {
        var carrito = HttpContext.Session.GetObjectFromJson<Carrito>("Carrito");

        if (carrito == null || !carrito.Items.Any())
        {
            return RedirectToAction("Index", "Carrito");
        }

        return View(carrito);
    }

    // POST: VENTAS/ConfirmarCompra
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarCompra(MetodoDePago metodoPago)
    {
        var carrito = HttpContext.Session.GetObjectFromJson<Carrito>("Carrito");

        if (carrito == null || !carrito.Items.Any())
        {
            return RedirectToAction("Index", "Carrito");
        }

        var nuevaVenta = new Venta
        {
            Fecha = DateTime.Now,
            MetodoPago = metodoPago,
            Total = carrito.Total,
            Cliente = carrito.Cliente // Asigna el cliente si ya está cargado en el carrito
        };

        foreach (var item in carrito.Items)
        {
            var dbProducto = await _context.Productos.FindAsync(item.Producto.Id);
            if (dbProducto != null)
            {
                if (dbProducto.Stock < item.Cantidad)
                {
                    ModelState.AddModelError("", $"No hay suficiente stock disponible para: {dbProducto.Nombre}. Stock actual: {dbProducto.Stock}");
                    return View("Checkout", carrito);
                }

                dbProducto.Stock -= item.Cantidad;
                _context.Productos.Update(dbProducto);

                nuevaVenta.Detalle.Add(new ItemCarrito
                {
                    Producto = dbProducto,
                    Cantidad = item.Cantidad
                });
            }
        }

        _context.Venta.Add(nuevaVenta);
        await _context.SaveChangesAsync();

        HttpContext.Session.Remove("Carrito");

        return RedirectToAction(nameof(Exito), new { id = nuevaVenta.Id });
    }

    // GET: VENTAS/Exito/5
    [HttpGet]
    public IActionResult Exito(int id)
    {
        ViewBag.VentaId = id;
        return View();
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Fecha,Total,MetodoPago")] Venta venta)
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