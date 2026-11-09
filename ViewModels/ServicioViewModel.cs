using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilaVirtual.Data;
using FilaVirtual.Models;
using FilaVirtual.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FilaVirtual.ViewModels;

public partial class ServicioViewModel : BaseViewModel
{
    private readonly SQLiteDataService _data;
    private readonly IAppDialogService _dialog;

    [ObservableProperty]
    private ObservableCollection<Servicio> servicios = new();

    [ObservableProperty]
    private int id;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "El nombre del servicio es obligatorio.")]
    [StringLength(
        100,
        ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    private string nombre = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Debes escribir una descripción.")]
    [StringLength(
        200,
        ErrorMessage = "La descripción no puede superar 200 caracteres.")]
    private string descripcion = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(
        1,
        1440,
        ErrorMessage = "La duración debe estar entre 1 y 1440 minutos.")]
    private int duracionMinutos;

    public bool EstaEditando => Id > 0;

    public string TituloFormulario =>
        EstaEditando
            ? "Editar servicio"
            : "Nuevo servicio";

    public string NombreError =>
        ObtenerError(nameof(Nombre));

    public string DescripcionError =>
        ObtenerError(nameof(Descripcion));

    public string DuracionMinutosError =>
        ObtenerError(nameof(DuracionMinutos));

    public ServicioViewModel(
        SQLiteDataService data,
        IAppDialogService dialog)
    {
        _data = data;
        _dialog = dialog;
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
                await _data.ObtenerTodosAsync<Servicio>();

            Servicios.Clear();

            foreach (var servicio in registros.OrderBy(s => s.Nombre))
            {
                Servicios.Add(servicio);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudieron cargar los servicios: {ex.Message}",
                "Aceptar");
        }
    }

    public async Task CargarServicioAsync(int servicioId)
    {
        try
        {
            await _data.InitializeAsync();

            var servicio =
                await _data.ObtenerPorIdAsync<Servicio>(servicioId);

            if (servicio is null)
            {
                await Shell.Current.DisplayAlert(
                    "Servicio no encontrado",
                    "No se encontró el servicio seleccionado.",
                    "Aceptar");

                return;
            }

            Id = servicio.Id;
            Nombre = servicio.Nombre;
            Descripcion = servicio.Descripcion;
            DuracionMinutos = servicio.DuracionMinutos;

            ClearErrors();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudo cargar el servicio: {ex.Message}",
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

        try
        {
            await _data.InitializeAsync();

            var servicio = new Servicio
            {
                Id = Id,
                Nombre = Nombre.Trim(),
                Descripcion = Descripcion.Trim(),
                DuracionMinutos = DuracionMinutos
            };

            if (EstaEditando)
            {
                await _data.ActualizarAsync(servicio);

                await _dialog.MostrarExitoAsync(
                    "El servicio fue actualizado correctamente.");
            }
            else
            {
                await _data.InsertarAsync(servicio);

                await _dialog.MostrarExitoAsync(
                    "El servicio fue registrado correctamente.");
            }

            LimpiarFormulario();

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudo guardar el servicio: {ex.Message}",
                "Aceptar");
        }
    }

    [RelayCommand]
    private async Task EliminarAsync(Servicio? servicio)
    {
        if (servicio is null)
            return;

        bool confirmar =
            await Shell.Current.DisplayAlert(
                "Eliminar servicio",
                $"¿Deseas eliminar el servicio {servicio.Nombre}?",
                "Eliminar",
                "Cancelar");

        if (!confirmar)
            return;

        try
        {
            await _data.InitializeAsync();
            await _data.EliminarAsync(servicio);

            Servicios.Remove(servicio);

            await _dialog.MostrarExitoAsync(
                "El servicio fue eliminado correctamente.");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudo eliminar el servicio: {ex.Message}",
                "Aceptar");
        }
    }

    [RelayCommand]
    private void LimpiarFormulario()
    {
        Id = 0;
        Nombre = string.Empty;
        Descripcion = string.Empty;
        DuracionMinutos = 0;

        ClearErrors();
    }
}