using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;

namespace PeluqueriaCanina
{
    public static class StartUp
    {
        public static WebApplication InicializarApp(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);
            var app = builder.Build();
            Configure(app);
            return app;
        }
        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<PeluqueriaContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("PeluqueriaContext")));

            builder.Services.AddControllersWithViews();
        }

        private static void Configure(WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        }
    }
}
