using ElectronNET.API;
using ElectronNET.API.Entities;
using Radzen;

namespace TreeViewer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseElectron(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                            .AddInteractiveServerComponents();
            builder.Services.AddRadzenComponents();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<Components.App>()
               .AddInteractiveServerRenderMode();

            BrowserWindow window = Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
            {
            }).Result;

            window.OnClosed += Electron.App.Quit;

            app.Run();
        }
    }
}
