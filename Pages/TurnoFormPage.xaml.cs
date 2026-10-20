using FilaVirtual.Models;
using FilaVirtual.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FilaVirtual.Pages;

public partial class TurnoFormPage : ContentPage
{
    private readonly MemoryDataService _data;

    public TurnoFormPage()
    {
        InitializeComponent();
        _data = Application.Current!.Handler!.MauiContext!.Services.GetRequiredService<MemoryDataService>();
        FechaPicker.Date = DateTime.Today;
        HoraPicker.Time = DateTime.Now.TimeOfDay;
        EstadoPicker.SelectedIndex = 0;
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
