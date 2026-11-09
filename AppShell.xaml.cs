<<<<<<< Updated upstream
using FilaVirtual.Pages;
=======
using FilaVirtual.Models;
using FilaVirtual.Pages;
using FilaVirtual.Services;
>>>>>>> Stashed changes

namespace FilaVirtual;

public partial class AppShell : Shell
{
    // Destinos que SOLO puede abrir un Admin. Ocultar el menú no basta: las rutas
    // registradas con Routing.RegisterRoute se pueden abrir desde cualquier código.
    private static readonly string[] RutasSoloAdmin =
    {
        "AdminMenu",
        "RegistroNegocioPage",
        "NegocioFormPage",
        "ServicioFormPage",
        "ClienteFormPage",
        "TurnoFormPage"
    };

    private readonly ISesionService _sesion;

    public AppShell(ISesionService sesion)
    {
        // Debe asignarse ANTES de InitializeComponent: el Shell hace su primera
        // navegación mientras se construye y OnNavigating ya usa _sesion.
        _sesion = sesion;

        InitializeComponent();

        Routing.RegisterRoute("TurnoFormPage", typeof(TurnoFormPage));
        Routing.RegisterRoute("NegocioFormPage", typeof(NegocioFormPage));
        Routing.RegisterRoute("ServicioFormPage", typeof(ServicioFormPage));
        Routing.RegisterRoute("ClienteFormPage", typeof(ClienteFormPage));
        Routing.RegisterRoute("QrScannerPage", typeof(QrScannerPage));
<<<<<<< Updated upstream
    }

    public async Task MostrarAplicacionAsync()
    {
        AppMenu.IsVisible = true;
        LogoutMenuItem.IsEnabled = true;
        FlyoutBehavior = Microsoft.Maui.FlyoutBehavior.Flyout;
        await GoToAsync("//AppMenu/HomePage");
=======
        Routing.RegisterRoute("RegistroNegocioPage", typeof(RegistroNegocioPage));
    }

    private static readonly string[] RutasSoloNegocio = { "NegocioMenu" };

    // Se llama desde LoginPage cuando la sesión ya está iniciada.
    public async Task MostrarAplicacionAsync()
    {
        var rol = _sesion.UsuarioActual?.Rol ?? RolUsuario.Cliente;

        AdminMenu.IsVisible = rol == RolUsuario.Admin;
        NegocioMenu.IsVisible = rol == RolUsuario.Negocio;
        ClienteMenu.IsVisible = rol == RolUsuario.Cliente;

        LogoutMenuItem.IsEnabled = true;
        FlyoutBehavior = Microsoft.Maui.FlyoutBehavior.Flyout;

        await GoToAsync(rol switch
        {
            RolUsuario.Admin => "//AdminMenu/NegociosPage",
            RolUsuario.Negocio => "//NegocioMenu/ColaPage",
            _ => "//ClienteMenu/HomePage"
        });
>>>>>>> Stashed changes
    }

    private async void OnCerrarSesionClicked(object sender, EventArgs e)
    {
<<<<<<< Updated upstream
        AppMenu.IsVisible = false;
        LogoutMenuItem.IsEnabled = false;
        FlyoutBehavior = Microsoft.Maui.FlyoutBehavior.Disabled;
        await GoToAsync("//LoginRoot/LoginPage");
    }
=======
        _sesion.Cerrar();

        AdminMenu.IsVisible = false;
        NegocioMenu.IsVisible = false;
        ClienteMenu.IsVisible = false;
        LogoutMenuItem.IsEnabled = false;
        FlyoutBehavior = Microsoft.Maui.FlyoutBehavior.Disabled;

        await GoToAsync("//LoginRoot/LoginPage");
    }

    // Guardia de navegación: cada rol solo entra a sus rutas.
    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        if (_sesion is null)
            return;

        string destino = args.Target?.Location.OriginalString ?? string.Empty;
        var rol = _sesion.UsuarioActual?.Rol;

        bool esRutaAdmin = RutasSoloAdmin.Any(r =>
            destino.Contains(r, StringComparison.OrdinalIgnoreCase));

        bool esRutaNegocio = RutasSoloNegocio.Any(r =>
            destino.Contains(r, StringComparison.OrdinalIgnoreCase));

        if (args.CanCancel &&
            ((esRutaAdmin && rol != RolUsuario.Admin) ||
             (esRutaNegocio && rol != RolUsuario.Negocio)))
        {
            args.Cancel();
        }
    }
>>>>>>> Stashed changes
}
