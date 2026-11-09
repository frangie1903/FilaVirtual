using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilaVirtual.Data;
using FilaVirtual.Models;
using FilaVirtual.Services;
using System.Collections.ObjectModel;

namespace FilaVirtual.ViewModels;

/// <summary>
/// CLIENTE: elige negocio -> sucursal -> tipo de diligencia y genera el ticket.
/// </summary>
public partial class NuevoTicketViewModel : BaseViewModel
{
    private readonly SQLiteDataService _data;
    private readonly TicketService _tickets;
    private readonly ISesionService _sesion;
    private readonly IAppDialogService _dialog;

    public ObservableCollection<Negocio> Negocios { get; } = new();
    public ObservableCollection<Sucursal> Sucursales { get; } = new();
    public ObservableCollection<Servicio> Servicios { get; } = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmarCommand))]
    private Negocio? negocioSeleccionado;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HorarioSucursal))]
    [NotifyCanExecuteChangedFor(nameof(ConfirmarCommand))]
    private Sucursal? sucursalSeleccionada;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmarCommand))]
    private Servicio? servicioSeleccionado;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayTicket))]
    private Turno? ticketGenerado;

    public bool HayTicket => TicketGenerado is not null;

    public string HorarioSucursal =>
        SucursalSeleccionada?.DescribirHorario() ?? string.Empty;

    public NuevoTicketViewModel(
        SQLiteDataService data,
        TicketService tickets,
        ISesionService sesion,
        IAppDialogService dialog)
    {
        _data = data;
        _tickets = tickets;
        _sesion = sesion;
        _dialog = dialog;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        try
        {
            var negocios = await _data.ObtenerTodosAsync<Negocio>();

            Negocios.Clear();

            foreach (var n in negocios.OrderBy(n => n.Nombre))
                Negocios.Add(n);

            NegocioSeleccionado = null;
            TicketGenerado = null;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error", $"No se pudieron cargar los negocios: {ex.Message}", "Aceptar");
        }
    }

    // Al elegir negocio se rellenan sus sucursales y sus tipos de diligencia.
    partial void OnNegocioSeleccionadoChanged(Negocio? value) =>
        _ = CargarDetalleNegocioAsync(value);

    private async Task CargarDetalleNegocioAsync(Negocio? negocio)
    {
        Sucursales.Clear();
        Servicios.Clear();
        SucursalSeleccionada = null;
        ServicioSeleccionado = null;

        if (negocio is null)
            return;

        int negocioId = negocio.Id;

        try
        {
            var sucursales = await _data.ObtenerDondeAsync<Sucursal>(
                s => s.NegocioId == negocioId);

            var enlaces = await _data.ObtenerDondeAsync<NegocioServicio>(
                x => x.NegocioId == negocioId);

            var todosServicios = await _data.ObtenerTodosAsync<Servicio>();

            // Si el usuario cambió de negocio mientras cargábamos, descartamos.
            if (NegocioSeleccionado?.Id != negocioId)
                return;

            foreach (var s in sucursales.OrderBy(s => s.Nombre))
                Sucursales.Add(s);

            // Si el negocio aún no tiene servicios asignados (negocios creados
            // antes de esta función) se muestran todos, para no dejar el flujo vacío.
            var ids = enlaces.Select(e => e.ServicioId).ToHashSet();

            var disponibles = ids.Count == 0
                ? todosServicios
                : todosServicios.Where(s => ids.Contains(s.Id)).ToList();

            foreach (var s in disponibles.OrderBy(s => s.Nombre))
                Servicios.Add(s);

            if (Sucursales.Count == 1)
                SucursalSeleccionada = Sucursales[0];

            if (Servicios.Count == 1)
                ServicioSeleccionado = Servicios[0];
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error", $"No se pudo cargar el negocio: {ex.Message}", "Aceptar");
        }
    }

    private bool PuedeConfirmar() =>
        NegocioSeleccionado is not null &&
        SucursalSeleccionada is not null &&
        ServicioSeleccionado is not null;

    // AsyncRelayCommand no permite ejecuciones simultáneas por defecto:
    // un doble toque no genera dos tickets.
    [RelayCommand(CanExecute = nameof(PuedeConfirmar))]
    private async Task ConfirmarAsync()
    {
        var usuario = _sesion.UsuarioActual;

        if (usuario is null)
        {
            await Shell.Current.DisplayAlert(
                "Sesión", "Vuelve a iniciar sesión para generar un ticket.", "Aceptar");
            return;
        }

        try
        {
            var ticket = await _tickets.GenerarAsync(
                usuario,
                NegocioSeleccionado!,
                SucursalSeleccionada!,
                ServicioSeleccionado!);

            TicketGenerado = ticket;

            await _dialog.MostrarExitoAsync(
                $"Tu ticket es {ticket.Numero}. " +
                $"Personas delante de ti: {ticket.PersonasDelante}.");
        }
        catch (InvalidOperationException ex)
        {
            // Reglas de negocio: sucursal cerrada, ticket duplicado...
            await Shell.Current.DisplayAlert(
                "No se pudo generar el ticket", ex.Message, "Aceptar");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error", $"No se pudo generar el ticket: {ex.Message}", "Aceptar");
        }
    }

    [RelayCommand]
    private Task VerMisTicketsAsync() =>
        Shell.Current.GoToAsync("//ClienteMenu/TurnosPage");
}
