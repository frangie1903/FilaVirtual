using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilaVirtual.Data;
using FilaVirtual.Models;
using FilaVirtual.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FilaVirtual.ViewModels;

public partial class TurnoViewModel : BaseViewModel
{
    private readonly SQLiteDataService _data;
    private readonly IAppDialogService _dialog;
    private readonly ISesionService _sesion;

    [ObservableProperty]
    private ObservableCollection<Turno> turnos = new();

    [ObservableProperty]
    private int id;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "El número del turno es obligatorio.")]
    [StringLength(
        10,
        ErrorMessage = "El número no puede superar 10 caracteres.")]
    private string numero = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Debes indicar el negocio.")]
    private string negocio = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Debes indicar el servicio.")]
    private string servicio = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Debes seleccionar el estado.")]
    private string estado = "En espera";

    [ObservableProperty]
    private DateTime fecha = DateTime.Today;

    [ObservableProperty]
    private TimeSpan hora = DateTime.Now.TimeOfDay;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Indica cuántas personas tienes delante.")]
    [RegularExpression(
        @"^\d+$",
        ErrorMessage = "Introduce un número entero válido.")]
    private string personasDelante = string.Empty;

    public bool EstaEditando => Id > 0;

    public string TituloFormulario =>
        EstaEditando
            ? "Editar turno"
            : "Nuevo turno";

    public string NumeroError =>
        ObtenerError(nameof(Numero));

    public string NegocioError =>
        ObtenerError(nameof(Negocio));

    public string ServicioError =>
        ObtenerError(nameof(Servicio));

    public string EstadoError =>
        ObtenerError(nameof(Estado));

    public string PersonasDelanteError =>
        ObtenerError(nameof(PersonasDelante));

    // Para mostrar/ocultar botones de Admin en la lista de tickets.
    public bool EsAdmin => _sesion.EsAdmin;

    public TurnoViewModel(
        SQLiteDataService data,
        IAppDialogService dialog,
        ISesionService sesion)
    {
        _data = data;
        _dialog = dialog;
        _sesion = sesion;
    }

    partial void OnIdChanged(int value)
    {
        OnPropertyChanged(nameof(EstaEditando));
        OnPropertyChanged(nameof(TituloFormulario));
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        try
        {
            await _data.InitializeAsync();

            var registros =
                await _data.ObtenerTodosAsync<Turno>();

            // El VM es singleton y el rol puede cambiar entre sesiones.
            OnPropertyChanged(nameof(EsAdmin));

            // El Admin ve todos los tickets; el Cliente solo los suyos.
            if (!_sesion.EsAdmin)
            {
                int usuarioId = _sesion.UsuarioActual?.Id ?? -1;

                registros = registros
                    .Where(t => t.UsuarioId == usuarioId)
                    .ToList();
            }

            Turnos.Clear();

            foreach (var turno in registros
                .OrderByDescending(t => t.Fecha)
                .ThenByDescending(t => t.Hora))
            {
                Turnos.Add(turno);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudieron cargar los turnos: {ex.Message}",
                "Aceptar");
        }
    }

    public async Task CargarTurnoAsync(int turnoId)
    {
        try
        {
            await _data.InitializeAsync();

            var turno =
                await _data.ObtenerPorIdAsync<Turno>(turnoId);

            if (turno is null)
            {
                await Shell.Current.DisplayAlert(
                    "Turno no encontrado",
                    "No se encontró el turno seleccionado.",
                    "Aceptar");

                return;
            }

            Id = turno.Id;
            Numero = turno.Numero;
            Negocio = turno.Negocio;
            Servicio = turno.Servicio;
            Estado = turno.Estado;
            Fecha = turno.Fecha;
            Hora = turno.Hora;
            PersonasDelante = turno.PersonasDelante.ToString();

            ClearErrors();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudo cargar el turno: {ex.Message}",
                "Aceptar");
        }
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            return;
        }

        if (!int.TryParse(PersonasDelante, out int personas) ||
            personas < 0)
        {
            return;
        }

        try
        {
            await _data.InitializeAsync();

            // Al EDITAR se parte del registro guardado. Si se creara un Turno
            // nuevo, UpdateAsync pondría en 0 las columnas que este formulario
            // no muestra (UsuarioId, SucursalId, Secuencia...) y el ticket
            // dejaría de aparecerle a su dueño.
            var turno = (EstaEditando
                ? await _data.ObtenerPorIdAsync<Turno>(Id)
                : null) ?? new Turno { Id = Id };

            turno.Numero = Numero.Trim();
            turno.Negocio = Negocio.Trim();
            turno.Servicio = Servicio.Trim();
            turno.Estado = Estado.Trim();
            turno.Fecha = Fecha.Date;
            turno.Hora = Hora;
            turno.PersonasDelante = personas;

            if (EstaEditando)
            {
                int resultado = await _data.ActualizarAsync(turno);

                if (resultado <= 0)
                {
                    await Shell.Current.DisplayAlert(
                        "Aviso",
                        "No se pudo actualizar el turno.",
                        "Aceptar");

                    return;
                }

                await CargarAsync();

                await _dialog.MostrarExitoAsync(
                    "El turno fue actualizado correctamente.");
            }
            else
            {
                int nuevoId = await _data.InsertarAsync(turno);

                if (nuevoId <= 0)
                {
                    await Shell.Current.DisplayAlert(
                        "Aviso",
                        "No se pudo registrar el turno en la base de datos.",
                        "Aceptar");

                    return;
                }

                turno.Id = nuevoId;

                // Recargar todos los turnos desde SQLite.
                await CargarAsync();

                await _dialog.MostrarExitoAsync(
                    "El turno fue registrado correctamente.");
            }

            LimpiarFormulario();

            await Shell.Current.GoToAsync(".."); ;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudo guardar el turno: {ex.Message}",
                "Aceptar");
        }
    }

    [RelayCommand]
    private async Task EliminarAsync(Turno? turno)
    {
        if (turno is null || !_sesion.EsAdmin)
            return;

        bool confirmar =
            await Shell.Current.DisplayAlert(
                "Eliminar turno",
                $"¿Deseas eliminar el turno {turno.Numero}?",
                "Eliminar",
                "Cancelar");

        if (!confirmar)
            return;

        try
        {
            await _data.InitializeAsync();
            await _data.EliminarAsync(turno);

            Turnos.Remove(turno);

            await _dialog.MostrarExitoAsync(
                "El turno fue eliminado correctamente.");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudo eliminar el turno: {ex.Message}",
                "Aceptar");
        }
    }

    [RelayCommand]
    private void LimpiarFormulario()
    {
        Id = 0;
        Numero = string.Empty;
        Negocio = string.Empty;
        Servicio = string.Empty;
        Estado = "En espera";
        Fecha = DateTime.Today;
        Hora = DateTime.Now.TimeOfDay;
        PersonasDelante = string.Empty;

        ClearErrors();
    }
}