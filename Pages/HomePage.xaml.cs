namespace FilaVirtual.Pages;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnMisTurnosClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//AppMenu/TurnosPage");

    private async void OnEscanearQrClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("QrScannerPage");

    private async void OnNuevoTurnoClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("TurnoFormPage");
}
