using FilaVirtual.ViewModels;

namespace FilaVirtual.Pages;

public partial class ColaNegocioPage : ContentPage
{
    private readonly ColaNegocioViewModel _viewModel;

    public ColaNegocioPage(ColaNegocioViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.CargarCommand.ExecuteAsync(null);
    }

    // Los botones viven dentro de la plantilla: su BindingContext es la fila (TicketCola).
    private void OnAtenderClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: TicketCola ticket })
            _viewModel.AtenderCommand.Execute(ticket);
    }

    private void OnFinalizarClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: TicketCola ticket })
            _viewModel.FinalizarCommand.Execute(ticket);
    }

    private void OnCancelarClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: TicketCola ticket })
            _viewModel.CancelarCommand.Execute(ticket);
    }
}
