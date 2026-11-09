<<<<<<< Updated upstream
using FilaVirtual.Services;
=======
using FilaVirtual.Data;
using FilaVirtual.Pages;
using FilaVirtual.Services;
using FilaVirtual.ViewModels;
>>>>>>> Stashed changes
using Microsoft.Extensions.Logging;

namespace FilaVirtual;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
<<<<<<< Updated upstream
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
=======
                fonts.AddFont(
                    "OpenSans-Regular.ttf",
                    "OpenSansRegular");

                fonts.AddFont(
                    "OpenSans-Semibold.ttf",
                    "OpenSansSemibold");
            });

        // BASE DE DATOS SQLITE
        // Singleton mantiene una sola conexión durante la aplicación.
        builder.Services.AddSingleton<SQLiteDataService>();

        // SESIÓN, AUTENTICACIÓN Y TICKETS
        builder.Services.AddSingleton<ISesionService, SesionService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<TicketService>();

        // SERVICIO DE ALERTAS
        builder.Services.AddSingleton<
            IAppDialogService,
            AppDialogService>();

        // VIEWMODELS
        builder.Services.AddTransient<ClienteViewModel>();
        builder.Services.AddTransient<NegocioViewModel>();
        builder.Services.AddTransient<ServicioViewModel>();
        builder.Services.AddTransient<RegistroNegocioViewModel>();
        builder.Services.AddTransient<NuevoTicketViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<ColaNegocioViewModel>();
        builder.Services.AddSingleton<TurnoViewModel>();

        // PÁGINAS DE LISTADOS
        builder.Services.AddTransient<ClientesPage>();
        builder.Services.AddTransient<NegociosPage>();
        builder.Services.AddTransient<ServiciosPage>();
        builder.Services.AddTransient<TurnosPage>();

        // PÁGINAS DE FORMULARIOS
        builder.Services.AddTransient<ClienteFormPage>();
        builder.Services.AddTransient<NegocioFormPage>();
        builder.Services.AddTransient<ServicioFormPage>();
        builder.Services.AddTransient<TurnoFormPage>();
        builder.Services.AddTransient<RegistroNegocioPage>();
        builder.Services.AddTransient<NuevoTicketPage>();
        builder.Services.AddTransient<ColaNegocioPage>();

        // SHELL (recibe ISesionService)
        builder.Services.AddTransient<AppShell>();

        // OTRAS PÁGINAS
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<QrScannerPage>();
>>>>>>> Stashed changes

        builder.Services.AddSingleton<MemoryDataService>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
        return builder.Build();
    }
}