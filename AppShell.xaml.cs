using FilaVirtual.Pages;

namespace FilaVirtual;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("TurnoFormPage", typeof(TurnoFormPage));
        Routing.RegisterRoute("NegocioFormPage", typeof(NegocioFormPage));
        Routing.RegisterRoute("ServicioFormPage", typeof(ServicioFormPage));
        Routing.RegisterRoute("ClienteFormPage", typeof(ClienteFormPage));
        Routing.RegisterRoute("QrScannerPage", typeof(QrScannerPage));
    }

    public async Task MostrarAplicacionAsync()
    {
        AppMenu.IsVisible = true;
        LogoutMenuItem.IsEnabled = true;
        FlyoutBehavior = Microsoft.Maui.FlyoutBehavior.Flyout;
        await GoToAsync("//AppMenu/HomePage");
    }

    private async void OnCerrarSesionClicked(object sender, EventArgs e)
    {
        AppMenu.IsVisible = false;
        LogoutMenuItem.IsEnabled = false;
        FlyoutBehavior = Microsoft.Maui.FlyoutBehavior.Disabled;
        await GoToAsync("//LoginRoot/LoginPage");
    }
}
