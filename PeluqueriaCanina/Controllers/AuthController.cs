using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models;
using System.Text.RegularExpressions;

namespace PeluqueriaCanina.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PeluqueriaContext _context;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            PeluqueriaContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: Auth/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null || !user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, password, isPersistent: false, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Redirigir según tipo de usuario
                    if (user.TipoUsuario == "Cliente")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (user.TipoUsuario == "Empleado")
                    {
                        return RedirectToAction("Index", "Personas");
                    }
                    return LocalRedirect(returnUrl ?? "/");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Cuenta bloqueada temporalmente. Intente más tarde.");
                    return View();
                }

                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al iniciar sesión: {ex.Message}");
            }

            return View();
        }

        // GET: Auth/Register
        [HttpGet]
        public IActionResult Register(string userType = null)
        {
            if (!string.IsNullOrEmpty(userType) && (userType != "Cliente" && userType != "Empleado"))
            {
                return BadRequest("Tipo de usuario inválido.");
            }
            ViewData["UserType"] = userType;
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string nombre,
            string apellido,
            string email,
            string dni,
            string telefono,
            string password,
            string confirmPassword,
            string userType)
        {
            ViewData["UserType"] = userType;

            if (string.IsNullOrEmpty(userType) || (userType != "Cliente" && userType != "Empleado"))
            {
                ModelState.AddModelError(string.Empty, "Tipo de usuario inválido.");
                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden.");
                return View();
            }

            // Validar campos
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(dni) || string.IsNullOrWhiteSpace(telefono))
            {
                ModelState.AddModelError(string.Empty, "Todos los campos son requeridos.");
                return View();
            }

            // Validar DNI (solo números, 7-8 dígitos)
            if (!Regex.IsMatch(dni, @"^[0-9]{7,8}$"))
            {
                ModelState.AddModelError(string.Empty, "DNI inválido (debe tener 7-8 números).");
                return View();
            }

            // Validar nombre y apellido (solo letras)
            if (!Regex.IsMatch(nombre, @"^[a-zA-Z áéíóúÁÉÍÓÚñÑ]*$") ||
                !Regex.IsMatch(apellido, @"^[a-zA-Z áéíóúÁÉÍÓÚñÑ]*$"))
            {
                ModelState.AddModelError(string.Empty, "Nombre y apellido deben contener solo letras.");
                return View();
            }

            try
            {
                // Verificar si el email ya existe
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "El email ya está registrado.");
                    return View();
                }

                // Verificar si el DNI ya existe
                var existingPersona = _context.Personas.FirstOrDefault(p => p.Dni == dni);
                if (existingPersona != null)
                {
                    ModelState.AddModelError(string.Empty, "El DNI ya está registrado.");
                    return View();
                }

                // Crear Persona según tipo
                Persona persona;
                if (userType == "Cliente")
                {
                    persona = new Cliente
                    {
                        Nombre = nombre,
                        Apellido = apellido,
                        Email = email,
                        Dni = dni,
                        Telefono = telefono,
                        NroCliente = GenerarNumeroCliente()
                    };
                }
                else // Empleado
                {
                    persona = new Empleado
                    {
                        Nombre = nombre,
                        Apellido = apellido,
                        Email = email,
                        Dni = dni,
                        Telefono = telefono,
                        Legajo = GenerarLegajo(),
                        Puesto = "Sin definir",
                        Sueldo = 0
                    };
                }

                _context.Personas.Add(persona);
                await _context.SaveChangesAsync();

                // Crear ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    PersonaId = persona.Id,
                    TipoUsuario = userType,
                    IsActive = true//userType == "Cliente" ? true : false // Empleados requieren activación
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Asignar rol
                    await _userManager.AddToRoleAsync(user, userType);

                    if (userType == "Cliente")
                    {
                        // Iniciar sesión automáticamente para clientes
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Para empleados, mostrar mensaje de espera
                        return RedirectToAction("RegistroExitoso");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al registrarse: {ex.Message}");
            }

            return View();
        }

        // GET: Auth/RegistroExitoso
        [HttpGet]
        public IActionResult RegistroExitoso()
        {
            return View();
        }

        // GET: Auth/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: Auth/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return View();
        }

        // POST: Auth/Logout (alternativa para formulario)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private int GenerarNumeroCliente()
        {
            var ultimoCliente = _context.Clientes.OrderByDescending(c => c.NroCliente).FirstOrDefault();
            return (ultimoCliente?.NroCliente ?? 0) + 1;
        }

        private int GenerarLegajo()
        {
            var ultimoEmpleado = _context.Empleados.OrderByDescending(e => e.Legajo).FirstOrDefault();
            return (ultimoEmpleado?.Legajo ?? 0) + 1;
        }
    }
}
