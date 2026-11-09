<<<<<<< Updated upstream
using FilaVirtual.Services;
=======
using FilaVirtual.Models;
using FilaVirtual.ViewModels;
>>>>>>> Stashed changes
using Microsoft.Extensions.DependencyInjection;

namespace FilaVirtual.Pages;

public partial class TurnosPage : ContentPage
{
<<<<<<< Updated upstream
    private readonly MemoryDataService _data;
=======
    private readonly TurnoViewModel _viewModel;
>>>>>>> Stashed changes

    public TurnosPage()
    {
        InitializeComponent();
<<<<<<< Updated upstream
        _data = Application.Current!.Handler!.MauiContext!.Services.GetRequiredService<MemoryDataService>();
        TurnosCollection.ItemsSource = _data.Turnos;
    }

    private async void OnNuevoTurnoClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("TurnoFormPage");
}
=======

        _viewModel = Application.Current!
            .Handler!
            .MauiContext!
            .Services
            .GetRequiredService<TurnoViewModel>();

        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.CargarCommand.ExecuteAsync(null);
    }

    private async void OnNuevoTurnoClicked(
        object? sender,
        EventArgs e)
    {
        // Admin: formulario manual de siempre. Cliente: flujo de ticket automático.
        await Shell.Current.GoToAsync(
            _viewModel.EsAdmin
                ? nameof(TurnoFormPage)
                : "//ClienteMenu/NuevoTicketPage");
    }

    private async void OnEditarTurnoClicked(
        object? sender,
        EventArgs e)
    {
        if (sender is not Button boton ||
            boton.BindingContext is not Turno turno)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(TurnoFormPage)}?id={turno.Id}");
    }

    private async void OnEliminarTurnoClicked(
        object? sender,
        EventArgs e)
    {
        if (sender is not Button boton ||
            boton.BindingContext is not Turno turno)
        {
            return;
        }

        await _viewModel
            .EliminarCommand
            .ExecuteAsync(turno);
    }
}
>>>>>>> Stashed changes
