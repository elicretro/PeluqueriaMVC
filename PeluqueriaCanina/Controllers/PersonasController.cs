
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Models;
using PeluqueriaCanina.Data;

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
        if (id == null)
        {
            return NotFound();
        }

        var persona = await _context.Personas
            .FirstOrDefaultAsync(m => m.Id == id);
        if (persona == null)
        {
            return NotFound();
        }

        return View(persona);
    }

    // GET: PERSONAS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PERSONAS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
     [Bind("Id,Nombre,Apellido,Dni,Telefono,Email")] Persona persona,
     string TipoPersona,
     string puestoInput,
     string especialidadInput)
    {
        if (ModelState.IsValid)
        {
            Persona nuevaPersona;

            if (TipoPersona == "Empleado")
            {
                TipoServicio especialidadConvertida;
                if (!Enum.TryParse(especialidadInput, out especialidadConvertida))
                {
                    especialidadConvertida = TipoServicio.Peluqueria;
                }

                nuevaPersona = new Empleado
                {
                    Nombre = persona.Nombre,
                    Apellido = persona.Apellido,
                    Dni = persona.Dni,
                    Telefono = persona.Telefono,
                    Email = persona.Email,
                    Puesto = !string.IsNullOrEmpty(puestoInput) ? puestoInput : "Staff", // Guardamos el texto libre escrito
                    Especialidad = especialidadConvertida // Guardamos el Enum
                };
            }
            else
            {
                nuevaPersona = new Cliente
                {
                    Nombre = persona.Nombre,
                    Apellido = persona.Apellido,
                    Dni = persona.Dni,
                    Telefono = persona.Telefono,
                    Email = persona.Email
                };
            }

            _context.Add(nuevaPersona);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
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
                            Puesto = !string.IsNullOrEmpty(puestoInput) ? puestoInput : "Staff",
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
                        empleadoEnDb.Puesto = !string.IsNullOrEmpty(puestoInput) ? puestoInput : "Staff";

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

    // POST: PERSONAS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var persona = await _context.Personas.FindAsync(id);
        if (persona != null)
        {
            _context.Personas.Remove(persona);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PersonaExists(int? id)
    {
        return _context.Personas.Any(e => e.Id == id);
    }
}
