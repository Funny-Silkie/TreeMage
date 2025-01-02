using ElectronNET.API;
using Radzen;
using TreeViewer.Models;
using TreeViewer.ViewModels;
using TreeViewer.Window;

namespace TreeViewer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseElectron(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                            .AddInteractiveServerComponents();
            builder.Services.AddRadzenComponents();
            builder.Services.AddScoped<MainModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<TreeEditSidebarViewModel>();
            builder.Services.AddTransient<StyleSidebarViewModel>();
            builder.Services.AddTransient<EditConfigViewModel>();
            builder.Services.AddTransient<VersionViewModel>();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                app.UseStaticFiles(new StaticFileOptions()
                {
                    OnPrepareResponse = context =>
                    {
                        context.Context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
                        context.Context.Response.Headers.Pragma = "no-cache";
                        context.Context.Response.Headers.Expires = "0";
                    },
                });
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<Components.App>()
               .AddInteractiveServerRenderMode();

            MainWindow window = MainWindow.Instance;
            window.InitMenubar();
            await window.CreateElectronWindow();

            app.Run();
        }
    }
}
