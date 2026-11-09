using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilaVirtual.Data;
using FilaVirtual.Models;
using FilaVirtual.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FilaVirtual.ViewModels;

public partial class NegocioViewModel : BaseViewModel
{
    private readonly SQLiteDataService _data;
    private readonly IAppDialogService _dialog;

    [ObservableProperty]
    private ObservableCollection<Negocio> negocios = new();

    [ObservableProperty]
    private int id;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "El nombre del negocio es obligatorio.")]
    [StringLength(
        80,
        ErrorMessage = "El nombre no puede superar 80 caracteres.")]
    private string nombre = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "La dirección del negocio es obligatoria.")]
    [StringLength(
        200,
        ErrorMessage = "La dirección no puede superar 200 caracteres.")]
    private string direccion = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(
        @"^[0-9+\-\s]{7,15}$",
        ErrorMessage = "Introduce un teléfono válido de 7 a 15 caracteres.")]
    private string telefono = string.Empty;

    public bool EstaEditando => Id > 0;

    public string TituloFormulario =>
        EstaEditando
            ? "Editar negocio"
            : "Nuevo negocio";

    public string NombreError =>
        ObtenerError(nameof(Nombre));

    public string DireccionError =>
        ObtenerError(nameof(Direccion));

    public string TelefonoError =>
        ObtenerError(nameof(Telefono));

    public NegocioViewModel(
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
                await _data.ObtenerTodosAsync<Negocio>();

            Negocios.Clear();

            foreach (var negocio in registros.OrderBy(n => n.Nombre))
            {
                Negocios.Add(negocio);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudieron cargar los negocios: {ex.Message}",
                "Aceptar");
        }
    }

    public async Task CargarNegocioAsync(int negocioId)
    {
        try
        {
            await _data.InitializeAsync();

            var negocio =
                await _data.ObtenerPorIdAsync<Negocio>(negocioId);

            if (negocio is null)
            {
                await Shell.Current.DisplayAlert(
                    "Negocio no encontrado",
                    "No se encontró el negocio seleccionado.",
                    "Aceptar");

                return;
            }

            Id = negocio.Id;
            Nombre = negocio.Nombre;
            Direccion = negocio.Direccion;
            Telefono = negocio.Telefono;

            ClearErrors();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudo cargar el negocio: {ex.Message}",
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

            var negocio = new Negocio
            {
                Id = Id,
                Nombre = Nombre.Trim(),
                Direccion = Direccion.Trim(),
                Telefono = Telefono.Trim()
            };

            if (EstaEditando)
            {
                await _data.ActualizarAsync(negocio);

                await _dialog.MostrarExitoAsync(
                    "El negocio fue actualizado correctamente.");
            }
            else
            {
                await _data.InsertarAsync(negocio);

                await _dialog.MostrarExitoAsync(
                    "El negocio fue registrado correctamente.");
            }

            LimpiarFormulario();

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudo guardar el negocio: {ex.Message}",
                "Aceptar");
        }
    }

    [RelayCommand]
    private async Task EliminarAsync(Negocio? negocio)
    {
        if (negocio is null)
            return;

        bool confirmar =
            await Shell.Current.DisplayAlert(
                "Eliminar negocio",
                $"¿Deseas eliminar el negocio {negocio.Nombre}?",
                "Eliminar",
                "Cancelar");

        if (!confirmar)
            return;

        try
        {
            await _data.InitializeAsync();
            await _data.EliminarAsync(negocio);

            Negocios.Remove(negocio);

            await _dialog.MostrarExitoAsync(
                "El negocio fue eliminado correctamente.");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudo eliminar el negocio: {ex.Message}",
                "Aceptar");
        }
    }

    [RelayCommand]
    private void LimpiarFormulario()
    {
        Id = 0;
        Nombre = string.Empty;
        Direccion = string.Empty;
        Telefono = string.Empty;

        ClearErrors();
    }
}