
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Models;
using PeluqueriaCanina.Data;

[Authorize]
public class PersonasController : Controller
{
    private readonly PeluqueriaContext _context;

    public PersonasController(PeluqueriaContext context)
    {
        _context = context;
    }

    // GET: PERSONAS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Personas.ToListAsync());
    }

    // GET: PERSONAS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        // Cargamos la persona e incluimos sus mascotas de forma explícita si es Cliente
        var persona = await _context.Personas
            .Include(p => (p as Cliente).Mascotas)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (persona == null) return NotFound();

        return View(persona);
    }

    // GET: Personas/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PERSONAS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Dni,Telefono,Email")] Persona persona, string TipoPersona, string puestoInput, string especialidadInput)
    {
        // Forzamos la remoción de validaciones extras que puedan estar molestando en el ModelState
        ModelState.Remove("puestoInput");
        ModelState.Remove("especialidadInput");

        if (ModelState.IsValid)
        {
            if (TipoPersona == "Empleado")
            {
                // Mapeo manual a tu clase Empleado
                TipoServicio especialidadEnum;
                Enum.TryParse(especialidadInput, out especialidadEnum);

                var nuevoEmpleado = new Empleado
                {
                    Nombre = persona.Nombre,
                    Apellido = persona.Apellido,
                    Dni = persona.Dni,
                    Telefono = persona.Telefono,
                    Email = persona.Email,
                    Puesto = TipoPuesto.Peluquero,
                    Especialidad = especialidadEnum
                };
                _context.Add(nuevoEmpleado);
            }
            else
            {
                // Mapeo manual a tu clase Cliente (con su lista de mascotas vacía al inicio)
                var nuevoCliente = new Cliente
                {
                    Nombre = persona.Nombre,
                    Apellido = persona.Apellido,
                    Dni = persona.Dni,
                    Telefono = persona.Telefono,
                    Email = persona.Email,
                    Mascotas = new List<Mascota>()
                };
                _context.Add(nuevoCliente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Si llegó acá, es porque falló. 
        // TIP: Poné un punto de interrupción (breakpoint) acá para revisar qué propiedad tiene el error en ModelState
        return View(persona);
    }

    // GET: PERSONAS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var persona = await _context.Personas.FindAsync(id);
        if (persona == null)
        {
            return NotFound();
        }
        return View(persona);
    }

    // POST: PERSONAS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
     int id,
     [Bind("Id,Nombre,Apellido,Dni,Telefono,Email")] Persona persona,
     string TipoPersona,
     string puestoInput,
     string especialidadInput)
    {
        if (id != persona.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var personaEnDb = await _context.Personas.FindAsync(id);
                if (personaEnDb == null)
                {
                    return NotFound();
                }

                bool esActualmenteCliente = personaEnDb is Cliente;
                bool quiereSerEmpleado = TipoPersona == "Empleado";

                // Si hay un CAMBIO DE ROL (Mutación)
                if ((esActualmenteCliente && quiereSerEmpleado) || (!esActualmenteCliente && !quiereSerEmpleado))
                {
                    _context.Personas.Remove(personaEnDb);
                    await _context.SaveChangesAsync();

                    Persona nuevaPersona;
                    if (quiereSerEmpleado)
                    {
                        TipoServicio especialidadConvertida;
                        if (!Enum.TryParse(especialidadInput, out especialidadConvertida))
                        {
                            especialidadConvertida = TipoServicio.Peluqueria;
                        }

                        nuevaPersona = new Empleado
                        {
                            Id = id,
                            Nombre = persona.Nombre,
                            Apellido = persona.Apellido,
                            Dni = persona.Dni,
                            Telefono = persona.Telefono,
                            Email = persona.Email,
                            Puesto = TipoPuesto.Peluquero,
                            Especialidad = especialidadConvertida
                        };
                    }
                    else
                    {
                        nuevaPersona = new Cliente
                        {
                            Id = id,
                            Nombre = persona.Nombre,
                            Apellido = persona.Apellido,
                            Dni = persona.Dni,
                            Telefono = persona.Telefono,
                            Email = persona.Email
                        };
                    }

                    _context.Personas.Add(nuevaPersona);
                }
                else
                {
                    // Si NO cambió el rol, actualizamos los datos comunes
                    personaEnDb.Nombre = persona.Nombre;
                    personaEnDb.Apellido = persona.Apellido;
                    personaEnDb.Dni = persona.Dni;
                    personaEnDb.Telefono = persona.Telefono;
                    personaEnDb.Email = persona.Email;

                    // Si se mantiene como empleado, actualizamos sus campos específicos
                    if (personaEnDb is Empleado empleadoEnDb)
                    {
                        empleadoEnDb.Puesto = TipoPuesto.Peluquero;

                        TipoServicio especialidadConvertida;
                        if (Enum.TryParse(especialidadInput, out especialidadConvertida))
                        {
                            empleadoEnDb.Especialidad = especialidadConvertida;
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonaExists(persona.Id))
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
        return View(persona);
    }

    // GET: Personas/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Buscamos a la persona en la base de datos
        var persona = await _context.Personas
            .FirstOrDefaultAsync(m => m.Id == id);

        if (persona == null)
        {
            return NotFound();
        }

        // Si la encuentra, le manda los datos a la vista Delete.cshtml
        return View(persona);
    }

    // POST: PERSONAS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var persona = await _context.Personas.FindAsync(id);

        if (persona != null)
        {
            // 1. Validamos si la persona que estamos intentando borrar es un Cliente
            if (persona is Cliente cliente)
            {
                // Cargamos las mascotas de la base de datos de forma explícita
                await _context.Entry(cliente).Collection(c => c.Mascotas).LoadAsync();

                // Si el cliente tiene aunque sea una mascota, tiramos el freno de mano
                if (cliente.Mascotas != null && cliente.Mascotas.Any())
                {
                    ModelState.AddModelError(string.Empty, "No se puede eliminar un cliente sin eliminar las mascotas primero.");

                    // Devolvemos la vista "Delete" con los datos para que muestre el error en el resumen
                    return View("Delete", persona);
                }
            }

            // 2. Si no entró al IF de arriba (porque es Empleado o Cliente sin mascotas), borra normalmente
            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // REINJECTÁ ESTE MÉTODO (que se voló al reemplazar el anterior)
    private bool PersonaExists(int? id)
    {
        return _context.Personas.Any(e => e.Id == id);
    }

} // <--- ESTA ÚLTIMA LLAVE CIERRA LA CLASE COMPLETAMENTE
