using ElectronNET.API;
using Radzen;
using System.Globalization;
using TreeMage.Models;
using TreeMage.Services;
using TreeMage.Settings;
using TreeMage.ViewModels;
using TreeMage.Window;

namespace TreeMage
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
#if TEST
            throw new NotSupportedException("Not supported on this configuration.");
#endif
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseElectron(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                            .AddInteractiveServerComponents();
            builder.Services.AddRadzenComponents();
            builder.Services.AddLocalization();

            builder.Services.AddScoped<MainModel>();
            builder.Services.AddScoped<StyleSidebarModel>();
            builder.Services.AddScoped<IElectronService>(_ => new ElectronService(MainWindow.Instance));
            builder.Services.AddKeyedScoped<IElectronService>("config", (_, _) => new ElectronService(EditConfigWindow.Instance));
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<TreeEditSidebarViewModel>();
            builder.Services.AddTransient<StyleSidebarViewModel>();
            builder.Services.AddTransient<EditConfigViewModel>();
            builder.Services.AddTransient<VersionViewModel>();

            Configurations config = await Configurations.LoadOrCreateAsync();
            CultureInfo.CurrentCulture = config.Culture;
            CultureInfo.CurrentUICulture = config.Culture;

            string[] supportedCultures = ["ja-JP", "en-US"];
            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(config.Culture.Name)
                                                                                             .AddSupportedCultures(supportedCultures)
                                                                                             .AddSupportedUICultures(supportedCultures);

            WebApplication app = builder.Build();
            app.UseRequestLocalization(localizationOptions);

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
