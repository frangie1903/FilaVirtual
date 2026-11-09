using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilaVirtual.Data;
using FilaVirtual.Models;
using FilaVirtual.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FilaVirtual.ViewModels;

public partial class ClienteViewModel : BaseViewModel
{
    private readonly SQLiteDataService _data;
    private readonly IAppDialogService _dialog;

    [ObservableProperty]
    private ObservableCollection<Cliente> clientes = new();

    [ObservableProperty]
    private int id;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(
        100,
        ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    private string nombre = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no es válido.")]
    private string correo = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(
        @"^[0-9+\-\s]{7,15}$",
        ErrorMessage = "El teléfono debe contener entre 7 y 15 caracteres.")]
    private string telefono = string.Empty;

    public bool EstaEditando => Id > 0;

    public string TituloFormulario =>
        EstaEditando
            ? "Editar cliente"
            : "Nuevo cliente";

    public string NombreError =>
        ObtenerError(nameof(Nombre));

    public string CorreoError =>
        ObtenerError(nameof(Correo));

    public string TelefonoError =>
        ObtenerError(nameof(Telefono));

    public ClienteViewModel(
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
                await _data.ObtenerTodosAsync<Cliente>();

            Clientes.Clear();

            foreach (var cliente in registros.OrderBy(c => c.Nombre))
            {
                Clientes.Add(cliente);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudieron cargar los clientes: {ex.Message}",
                "Aceptar");
        }
    }

    public async Task CargarClienteAsync(int clienteId)
    {
        try
        {
            await _data.InitializeAsync();

            var cliente =
                await _data.ObtenerPorIdAsync<Cliente>(clienteId);

            if (cliente is null)
            {
                await Shell.Current.DisplayAlert(
                    "Cliente no encontrado",
                    "No se encontró el cliente seleccionado.",
                    "Aceptar");

                return;
            }

            Id = cliente.Id;
            Nombre = cliente.Nombre;
            Correo = cliente.Correo;
            Telefono = cliente.Telefono;

            ClearErrors();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudo cargar el cliente: {ex.Message}",
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

            var cliente = new Cliente
            {
                Id = Id,
                Nombre = Nombre.Trim(),
                Correo = Correo.Trim().ToLowerInvariant(),
                Telefono = Telefono.Trim()
            };

            if (EstaEditando)
            {
                await _data.ActualizarAsync(cliente);

                await _dialog.MostrarExitoAsync(
                    "El cliente fue actualizado correctamente.");
            }
            else
            {
                await _data.InsertarAsync(cliente);

                await _dialog.MostrarExitoAsync(
                    "El cliente fue registrado correctamente.");
            }

            Limpiar();

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudo guardar el cliente: {ex.Message}",
                "Aceptar");
        }
    }

    [RelayCommand]
    private async Task EliminarAsync(Cliente? cliente)
    {
        if (cliente is null)
            return;

        bool confirmar =
            await Shell.Current.DisplayAlert(
                "Eliminar cliente",
                $"¿Deseas eliminar a {cliente.Nombre}?",
                "Eliminar",
                "Cancelar");

        if (!confirmar)
            return;

        try
        {
            await _data.InitializeAsync();
            await _data.EliminarAsync(cliente);

            Clientes.Remove(cliente);

            await _dialog.MostrarExitoAsync(
                "El cliente fue eliminado correctamente.");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"No se pudo eliminar el cliente: {ex.Message}",
                "Aceptar");
        }
    }

    [RelayCommand]
    private void Limpiar()
    {
        Id = 0;
        Nombre = string.Empty;
        Correo = string.Empty;
        Telefono = string.Empty;

        ClearErrors();
    }
}