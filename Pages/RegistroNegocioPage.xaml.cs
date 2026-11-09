using FilaVirtual.ViewModels;

namespace FilaVirtual.Pages;

public partial class RegistroNegocioPage : ContentPage, IQueryAttributable
{
    private readonly RegistroNegocioViewModel _viewModel;
    private bool _inicializada;

    public RegistroNegocioPage(RegistroNegocioViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    // Llega "?id=5" al editar; sin id es un alta.
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        int id = 0;

        if (query.TryGetValue("id", out var valor))
            int.TryParse(valor?.ToString(), out id);

        _inicializada = true;
        await _viewModel.InicializarAsync(id);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Alta sin parámetros: se inicializa una sola vez para no borrar
        // lo escrito si la página vuelve a aparecer.
        if (_inicializada)
            return;

        _inicializada = true;
        await _viewModel.InicializarAsync();
    }

    // El botón vive dentro de un BindableLayout: su BindingContext es la fila
    // (SucursalEditable).
    private void OnQuitarSucursalClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: SucursalEditable sucursal })
            _viewModel.QuitarSucursalCommand.Execute(sucursal);
    }

    private async void OnCancelarClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");
}
