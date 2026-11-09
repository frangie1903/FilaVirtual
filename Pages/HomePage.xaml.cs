<<<<<<< Updated upstream
=======
using FilaVirtual.ViewModels;

>>>>>>> Stashed changes
namespace FilaVirtual.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    // La página se queda en memoria dentro del Shell: recargar cada vez que
    // aparece mantiene la tarjeta sincronizada con los tickets creados.
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.CargarCommand.ExecuteAsync(null);
    }

    private async void OnMisTurnosClicked(object sender, EventArgs e)
<<<<<<< Updated upstream
        => await Shell.Current.GoToAsync("//AppMenu/TurnosPage");
=======
        => await Shell.Current.GoToAsync("//ClienteMenu/TurnosPage");
>>>>>>> Stashed changes

    private async void OnEscanearQrClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("QrScannerPage");

    private async void OnNuevoTurnoClicked(object sender, EventArgs e)
<<<<<<< Updated upstream
        => await Shell.Current.GoToAsync("TurnoFormPage");
=======
        => await Shell.Current.GoToAsync("//ClienteMenu/NuevoTicketPage");
>>>>>>> Stashed changes
}
