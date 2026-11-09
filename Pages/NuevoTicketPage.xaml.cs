using FilaVirtual.ViewModels;

namespace FilaVirtual.Pages;

public partial class NuevoTicketPage : ContentPage
{
    private readonly NuevoTicketViewModel _viewModel;

    public NuevoTicketPage(NuevoTicketViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Recarga los negocios cada vez que el cliente entra a esta pantalla.
        await _viewModel.CargarCommand.ExecuteAsync(null);
    }
}
