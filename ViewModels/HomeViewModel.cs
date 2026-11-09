using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilaVirtual.Data;
using FilaVirtual.Models;
using FilaVirtual.Services;

namespace FilaVirtual.ViewModels;

/// <summary>
/// Alimenta la tarjeta "Tu turno actual" de la pantalla de inicio con el
/// ticket activo REAL del cliente (el más reciente en espera o en atención).
/// </summary>
public partial class HomeViewModel : BaseViewModel
{
    private const int MinutosPorDefecto = 10;

    private readonly SQLiteDataService _data;
    private readonly ISesionService _sesion;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayTicket))]
    private Turno? ticket;

    [ObservableProperty] private string numero = "--";
    [ObservableProperty] private string estado = string.Empty;
    [ObservableProperty] private string personasTexto = "Sin ticket activo";
    [ObservableProperty] private string negocioTexto = "Genera uno con «Nuevo turno»";
    [ObservableProperty] private string horaTexto = string.Empty;
    [ObservableProperty] private Color estadoFondo = Color.FromArgb("#DDFBE5");
    [ObservableProperty] private Color estadoColor = Color.FromArgb("#15803D");

    public bool HayTicket => Ticket is not null;

    public HomeViewModel(SQLiteDataService data, ISesionService sesion)
    {
        _data = data;
        _sesion = sesion;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        try
        {
            int usuarioId = _sesion.UsuarioActual?.Id ?? -1;

            var activos = await _data.ObtenerDondeAsync<Turno>(t =>
                t.UsuarioId == usuarioId &&
                (t.Estado == EstadoTurno.EnEspera || t.Estado == EstadoTurno.Atendiendo));

            var actual = activos
                .OrderByDescending(t => t.Fecha)
                .ThenByDescending(t => t.Id)
                .FirstOrDefault();

            if (actual is null)
            {
                MostrarSinTicket();
                return;
            }

            bool atendiendo = actual.Estado == EstadoTurno.Atendiendo;

            // Personas delante calculadas AHORA: si el admin completa o cancela
            // tickets anteriores, el número baja solo.
            int delante = actual.PersonasDelante;

            if (actual.SucursalId > 0)
            {
                int sucursalId = actual.SucursalId;
                DateTime fecha = actual.Fecha;
                int secuencia = actual.Secuencia;

                var cola = await _data.ObtenerDondeAsync<Turno>(t =>
                    t.SucursalId == sucursalId &&
                    t.Fecha == fecha &&
                    t.Estado == EstadoTurno.EnEspera &&
                    t.Secuencia < secuencia);

                delante = cola.Count;
            }

            if (atendiendo)
                delante = 0;

            // Estimación: personas delante x duración del servicio.
            int minutosPorPersona = MinutosPorDefecto;

            if (actual.ServicioId > 0)
            {
                var servicio = await _data.ObtenerPorIdAsync<Servicio>(actual.ServicioId);

                if (servicio is not null && servicio.DuracionMinutos > 0)
                    minutosPorPersona = servicio.DuracionMinutos;
            }

            DateTime estimada = DateTime.Now.AddMinutes(delante * minutosPorPersona);

            Ticket = actual;
            Numero = actual.Numero;
            Estado = actual.Estado;

            PersonasTexto = atendiendo ? "Te están atendiendo"
                : delante == 0 ? "Eres el siguiente"
                : delante == 1 ? "1 persona delante"
                : $"{delante} personas delante";

            NegocioTexto = string.IsNullOrWhiteSpace(actual.Sucursal)
                ? actual.Negocio
                : $"{actual.Negocio} · {actual.Sucursal}";

            HoraTexto = atendiendo
                ? "En atención ahora"
                : $"Hora estimada: {estimada:h:mm tt}";

            EstadoFondo = Color.FromArgb(atendiendo ? "#FEF0C7" : "#DDFBE5");
            EstadoColor = Color.FromArgb(atendiendo ? "#B54708" : "#15803D");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al cargar el ticket activo: {ex.Message}");
            MostrarSinTicket();
        }
    }

    private void MostrarSinTicket()
    {
        Ticket = null;
        Numero = "--";
        Estado = string.Empty;
        PersonasTexto = "Sin ticket activo";
        NegocioTexto = "Genera uno con «Nuevo turno»";
        HoraTexto = string.Empty;
    }
}
