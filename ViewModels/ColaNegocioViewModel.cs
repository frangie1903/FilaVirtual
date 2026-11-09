using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilaVirtual.Data;
using FilaVirtual.Models;
using FilaVirtual.Services;
using System.Collections.ObjectModel;

namespace FilaVirtual.ViewModels;

// Fila de la cola: envuelve un Turno con los textos/estados ya calculados.
// La lista se reconstruye después de cada acción, por eso no necesita notificar cambios.
public class TicketCola
{
    public TicketCola(Turno turno) => Turno = turno;

    public Turno Turno { get; }

    public string Numero => Turno.Numero;
    public string Estado => Turno.Estado;

    public string Detalle => string.IsNullOrWhiteSpace(Turno.Sucursal)
        ? Turno.Servicio
        : $"{Turno.Servicio} · {Turno.Sucursal}";

    public string HoraTexto => $"Llegó a las {DateTime.Today.Add(Turno.Hora):h:mm tt}";

    public bool PuedeAtender => Estado == EstadoTurno.EnEspera;
    public bool PuedeFinalizar => Estado == EstadoTurno.Atendiendo;
    public bool PuedeCancelar =>
        Estado == EstadoTurno.EnEspera || Estado == EstadoTurno.Atendiendo;

    public Color EstadoFondo => Color.FromArgb(Estado switch
    {
        EstadoTurno.Atendiendo => "#FEF0C7",
        EstadoTurno.Completado => "#E0EDFF",
        EstadoTurno.Cancelado => "#FEE4E2",
        _ => "#DDFBE5"
    });

    public Color EstadoColor => Color.FromArgb(Estado switch
    {
        EstadoTurno.Atendiendo => "#B54708",
        EstadoTurno.Completado => "#087BF5",
        EstadoTurno.Cancelado => "#B42318",
        _ => "#15803D"
    });
}

/// <summary>
/// NEGOCIO (operador): ve la cola de HOY de su negocio y marca cada turno
/// como atendiendo, finalizado o cancelado.
/// </summary>
public partial class ColaNegocioViewModel : BaseViewModel
{
    private readonly SQLiteDataService _data;
    private readonly ISesionService _sesion;

    public ObservableCollection<TicketCola> Tickets { get; } = new();

    [ObservableProperty] private string titulo = "Cola de turnos";
    [ObservableProperty] private string resumen = string.Empty;

    public ColaNegocioViewModel(SQLiteDataService data, ISesionService sesion)
    {
        _data = data;
        _sesion = sesion;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        var usuario = _sesion.UsuarioActual;

        if (usuario is null || usuario.Rol != RolUsuario.Negocio)
            return;

        try
        {
            int negocioId = usuario.NegocioId;
            int sucursalId = usuario.SucursalId;
            DateTime hoy = DateTime.Today;

            var negocio = await _data.ObtenerPorIdAsync<Negocio>(negocioId);
            Titulo = negocio?.Nombre ?? "Cola de turnos";

            if (sucursalId > 0)
            {
                var sucursal = await _data.ObtenerPorIdAsync<Sucursal>(sucursalId);

                if (sucursal is not null)
                    Titulo = $"{Titulo} · {sucursal.Nombre}";
            }

            var turnos = await _data.ObtenerDondeAsync<Turno>(
                t => t.NegocioId == negocioId && t.Fecha == hoy);

            // Cada operador ve SOLO su sucursal. (SucursalId = 0 = acceso antiguo
            // de todo el negocio, que sigue viendo todas.)
            if (sucursalId > 0)
                turnos = turnos.Where(t => t.SucursalId == sucursalId).ToList();

            // Primero los que se están atendiendo, luego los que esperan, al final los cerrados.
            static int Prioridad(string estado) =>
                estado == EstadoTurno.Atendiendo ? 0 :
                estado == EstadoTurno.EnEspera ? 1 : 2;

            Tickets.Clear();

            foreach (var t in turnos
                .OrderBy(t => Prioridad(t.Estado))
                .ThenBy(t => t.Sucursal)
                .ThenBy(t => t.Secuencia))
            {
                Tickets.Add(new TicketCola(t));
            }

            int espera = turnos.Count(t => t.Estado == EstadoTurno.EnEspera);
            int atendiendo = turnos.Count(t => t.Estado == EstadoTurno.Atendiendo);
            int finalizados = turnos.Count(t => t.Estado == EstadoTurno.Completado);

            Resumen = $"En espera: {espera} · Atendiendo: {atendiendo} · Finalizados hoy: {finalizados}";
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error", $"No se pudo cargar la cola: {ex.Message}", "Aceptar");
        }
    }

    [RelayCommand]
    private Task AtenderAsync(TicketCola? item) =>
        CambiarEstadoAsync(item, EstadoTurno.Atendiendo, null,
            EstadoTurno.EnEspera);

    [RelayCommand]
    private Task FinalizarAsync(TicketCola? item) =>
        CambiarEstadoAsync(item, EstadoTurno.Completado, "Finalizar turno",
            EstadoTurno.Atendiendo);

    [RelayCommand]
    private Task CancelarAsync(TicketCola? item) =>
        CambiarEstadoAsync(item, EstadoTurno.Cancelado, "Cancelar turno",
            EstadoTurno.EnEspera, EstadoTurno.Atendiendo);

    private async Task CambiarEstadoAsync(
        TicketCola? item,
        string nuevoEstado,
        string? tituloConfirmacion,
        params string[] estadosPermitidos)
    {
        var usuario = _sesion.UsuarioActual;

        if (item is null || usuario is null || usuario.Rol != RolUsuario.Negocio)
            return;

        if (tituloConfirmacion is not null)
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                tituloConfirmacion,
                $"¿Seguro que quieres marcar el ticket {item.Numero} como \"{nuevoEstado}\"?",
                "Sí", "No");

            if (!confirmar)
                return;
        }

        int turnoId = item.Turno.Id;
        int negocioId = usuario.NegocioId;
        int sucursalId = usuario.SucursalId;

        try
        {
            // El estado se vuelve a leer DENTRO de la transacción: si otro
            // dispositivo ya lo cambió, no se pisa.
            await _data.EjecutarEnTransaccionAsync(db =>
            {
                var turno = db.Find<Turno>(turnoId);

                if (turno is null ||
                    turno.NegocioId != negocioId ||
                    (sucursalId > 0 && turno.SucursalId != sucursalId))
                {
                    throw new InvalidOperationException("Este ticket no pertenece a tu sucursal.");
                }

                if (!estadosPermitidos.Contains(turno.Estado))
                {
                    throw new InvalidOperationException(
                        $"El ticket {turno.Numero} ya está \"{turno.Estado}\". Se actualizó la lista.");
                }

                turno.Estado = nuevoEstado;

                if (nuevoEstado == EstadoTurno.Atendiendo)
                    turno.AtendidoEn = DateTime.Now;
                else
                    turno.FinalizadoEn = DateTime.Now;

                db.Update(turno);
            });
        }
        catch (InvalidOperationException ex)
        {
            await Shell.Current.DisplayAlert("No se pudo actualizar", ex.Message, "Aceptar");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error", $"No se pudo actualizar el ticket: {ex.Message}", "Aceptar");
        }

        await CargarAsync();
    }
}
