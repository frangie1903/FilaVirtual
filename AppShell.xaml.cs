namespace FilaVirtual;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("HomePage", typeof(Pages.HomePage));
        Routing.RegisterRoute("TurnosPage", typeof(Pages.TurnosPage));
        Routing.RegisterRoute("TurnoFormPage", typeof(Pages.TurnoFormPage));
    }
}
