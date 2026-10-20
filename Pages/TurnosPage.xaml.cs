using FilaVirtual.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FilaVirtual.Pages;

public partial class TurnosPage : ContentPage
{
    private readonly MemoryDataService _data;

    public TurnosPage()
    {
        InitializeComponent();
        _data = Application.Current!.Handler!.MauiContext!.Services.GetRequiredService<MemoryDataService>();
        TurnosCollection.ItemsSource = _data.Turnos;
    }

    private async void OnNuevoTurnoClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("TurnoFormPage");
}
