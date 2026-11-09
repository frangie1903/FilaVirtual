<<<<<<< Updated upstream
using FilaVirtual.Models;
using FilaVirtual.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FilaVirtual.Pages;

public class ClienteFormPage : ContentPage
{
    public ClienteFormPage()
    {
        Title = "Nuevo cliente";
        BackgroundColor = Color.FromArgb("#F5F7FC");
        var nombre = NegocioFormPage.Field("Nombre completo", "Nombre del cliente");
        var correo = NegocioFormPage.Field("Correo electrónico", "cliente@correo.com");
        correo.Keyboard = Keyboard.Email;
        var telefono = NegocioFormPage.Field("Teléfono", "809-555-0100");
        var save = NegocioFormPage.SaveButton();
        save.Clicked += async (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(nombre.Text) || string.IsNullOrWhiteSpace(correo.Text) || string.IsNullOrWhiteSpace(telefono.Text))
            { await DisplayAlert("Campos obligatorios", "Completa nombre, correo y teléfono.", "Aceptar"); return; }
            if (!correo.Text.Contains('@'))
            { await DisplayAlert("Correo inválido", "Escribe un correo electrónico válido.", "Aceptar"); return; }
            var data = Application.Current!.Handler!.MauiContext!.Services.GetRequiredService<MemoryDataService>();
            data.AddCliente(new Cliente { Nombre = nombre.Text.Trim(), Correo = correo.Text.Trim(), Telefono = telefono.Text.Trim() });
            await DisplayAlert("Guardado", "El cliente se guardó en memoria.", "Aceptar");
            await Shell.Current.GoToAsync("//AppMenu/ClientesPage");
        };
        Content = NegocioFormPage.FormLayout("Nuevo cliente", "Registra los datos del cliente", nombre, correo, telefono, save);
    }
}
=======
using FilaVirtual.ViewModels;

namespace FilaVirtual.Pages;

public class ClienteFormPage :
    ContentPage,
    IQueryAttributable
{
    private readonly ClienteViewModel _viewModel;

    public ClienteFormPage(ClienteViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = viewModel;

        Title = "Cliente";
        BackgroundColor = Color.FromArgb("#F5F7FC");

        var titulo = new Label
        {
            FontSize = 26,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#101828")
        };

        titulo.SetBinding(
            Label.TextProperty,
            nameof(ClienteViewModel.TituloFormulario));

        var nombreLabel = new Label
        {
            Text = "Nombre",
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#344054")
        };

        var nombreEntry = new Entry
        {
            Placeholder = "Nombre del cliente",
            BackgroundColor = Colors.White
        };

        nombreEntry.SetBinding(
            Entry.TextProperty,
            nameof(ClienteViewModel.Nombre));

        var nombreError = new Label
        {
            FontSize = 12,
            TextColor = Colors.Red
        };

        nombreError.SetBinding(
            Label.TextProperty,
            nameof(ClienteViewModel.NombreError));

        var correoLabel = new Label
        {
            Text = "Correo electrónico",
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#344054")
        };

        var correoEntry = new Entry
        {
            Placeholder = "correo@ejemplo.com",
            Keyboard = Keyboard.Email,
            BackgroundColor = Colors.White
        };

        correoEntry.SetBinding(
            Entry.TextProperty,
            nameof(ClienteViewModel.Correo));

        var correoError = new Label
        {
            FontSize = 12,
            TextColor = Colors.Red
        };

        correoError.SetBinding(
            Label.TextProperty,
            nameof(ClienteViewModel.CorreoError));

        var telefonoLabel = new Label
        {
            Text = "Teléfono",
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#344054")
        };

        var telefonoEntry = new Entry
        {
            Placeholder = "809-000-0000",
            Keyboard = Keyboard.Telephone,
            BackgroundColor = Colors.White
        };

        telefonoEntry.SetBinding(
            Entry.TextProperty,
            nameof(ClienteViewModel.Telefono));

        var telefonoError = new Label
        {
            FontSize = 12,
            TextColor = Colors.Red
        };

        telefonoError.SetBinding(
            Label.TextProperty,
            nameof(ClienteViewModel.TelefonoError));

        var guardarButton = new Button
        {
            Text = "Guardar",
            BackgroundColor = Color.FromArgb("#087BF5"),
            TextColor = Colors.White,
            CornerRadius = 12,
            HeightRequest = 50,
            Margin = new Thickness(0, 10, 0, 0)
        };

        guardarButton.SetBinding(
            Button.CommandProperty,
            nameof(ClienteViewModel.GuardarCommand));

        var cancelarButton = new Button
        {
            Text = "Cancelar",
            BackgroundColor = Color.FromArgb("#E5E7EB"),
            TextColor = Color.FromArgb("#344054"),
            CornerRadius = 12,
            HeightRequest = 50
        };

        cancelarButton.Clicked += OnCancelarClicked;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 25,
                Spacing = 10,
                Children =
                {
                    titulo,

                    nombreLabel,
                    nombreEntry,
                    nombreError,

                    correoLabel,
                    correoEntry,
                    correoError,

                    telefonoLabel,
                    telefonoEntry,
                    telefonoError,

                    guardarButton,
                    cancelarButton
                }
            }
        };
    }

    public async void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        _viewModel.LimpiarCommand.Execute(null);

        if (query.TryGetValue("id", out object? valor) &&
            int.TryParse(valor?.ToString(), out int clienteId))
        {
            await _viewModel.CargarClienteAsync(clienteId);
        }
    }

    private async void OnCancelarClicked(
        object? sender,
        EventArgs e)
    {
        _viewModel.LimpiarCommand.Execute(null);

        await Shell.Current.GoToAsync("..");
    }
}
>>>>>>> Stashed changes
