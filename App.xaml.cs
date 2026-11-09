using FilaVirtual.Data;
using Microsoft.Extensions.DependencyInjection;

namespace FilaVirtual;

public partial class App : Application
{
    private readonly SQLiteDataService _database;
    private readonly IServiceProvider _services;

    public App(SQLiteDataService database, IServiceProvider services)
    {
        InitializeComponent();

        _database = database;
        _services = services;
    }

    protected override Window CreateWindow(
        IActivationState? activationState)
    {
        InicializarBaseDeDatos();

        // AppShell ahora recibe ISesionService, así que se crea desde DI.
        return new Window(_services.GetRequiredService<AppShell>());
    }

    private async void InicializarBaseDeDatos()
    {
        try
        {
            await _database.InitializeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Error al inicializar SQLite: {ex.Message}");

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Current!.Windows[0].Page!.DisplayAlert(
                    "Error de base de datos",
                    "No se pudo inicializar la base de datos local.",
                    "Aceptar");
            });
        }
    }
}