<<<<<<< Updated upstream
using FilaVirtual.Models;
using FilaVirtual.Services;
using Microsoft.Extensions.DependencyInjection;
=======
using FilaVirtual.ViewModels;
>>>>>>> Stashed changes

namespace FilaVirtual.Pages;

public partial class TurnoFormPage :
    ContentPage,
    IQueryAttributable
{
<<<<<<< Updated upstream
    private readonly MemoryDataService _data;

    public TurnoFormPage()
    {
        InitializeComponent();
        _data = Application.Current!.Handler!.MauiContext!.Services.GetRequiredService<MemoryDataService>();
        FechaPicker.Date = DateTime.Today;
        HoraPicker.Time = DateTime.Now.TimeOfDay;
        EstadoPicker.SelectedIndex = 0;
=======
    private readonly TurnoViewModel _viewModel;

    public TurnoFormPage(TurnoViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public async void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        _viewModel.LimpiarFormularioCommand.Execute(null);

        if (query.TryGetValue("id", out object? valor) &&
            int.TryParse(valor?.ToString(), out int id))
        {
            await _viewModel.CargarTurnoAsync(id);
        }
    }

    private async void OnCancelarClicked(
        object? sender,
        EventArgs e)
    {
        _viewModel.LimpiarFormularioCommand.Execute(null);

        await Shell.Current.GoToAsync("..");
>>>>>>> Stashed changes
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NumeroEntry.Text) ||
            string.IsNullOrWhiteSpace(NegocioEntry.Text) ||
            string.IsNullOrWhiteSpace(ServicioEntry.Text) ||
            string.IsNullOrWhiteSpace(PersonasEntry.Text) ||
            EstadoPicker.SelectedItem is null)
        {
            await DisplayAlert("Campos obligatorios", "Completa todos los campos antes de guardar.", "Aceptar");
            return;
        }

        if (!int.TryParse(PersonasEntry.Text, out var personas) || personas < 0)
        {
            await DisplayAlert("Dato inválido", "Personas delante debe ser un número igual o mayor que cero.", "Aceptar");
            return;
        }

        _data.AddTurno(new Turno
        {
            Numero = NumeroEntry.Text.Trim(),
            Negocio = NegocioEntry.Text.Trim(),
            Servicio = ServicioEntry.Text.Trim(),
            Fecha = FechaPicker.Date ?? DateTime.Today,
            Hora = HoraPicker.Time ?? DateTime.Now.TimeOfDay,
            PersonasDelante = personas,
            Estado = EstadoPicker.SelectedItem.ToString()!
        });

        await DisplayAlert("Turno guardado", "El turno se agregó al listado en memoria.", "Aceptar");
        await Shell.Current.GoToAsync("//AppMenu/TurnosPage");
    }
}
