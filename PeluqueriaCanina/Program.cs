using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Services;

namespace PeluqueriaCanina
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var app = StartUp.InicializarApp(args);

            // Inicializar datos de la aplicación (roles, usuario admin, etc.)
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    await SeedData.InitializeAsync(scope.ServiceProvider);
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Error al inicializar datos de la aplicación");
                }
            }

            app.Run();
        }
    }
}
