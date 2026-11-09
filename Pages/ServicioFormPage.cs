<<<<<<< Updated upstream
using FilaVirtual.Models;
using FilaVirtual.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FilaVirtual.Pages;

public class ServicioFormPage : ContentPage
{
    public ServicioFormPage()
    {
        Title = "Nuevo servicio";
        BackgroundColor = Color.FromArgb("#F5F7FC");
        var nombre = NegocioFormPage.Field("Nombre del servicio", "Consulta general");
        var descripcion = NegocioFormPage.Field("Descripción", "Describe el servicio");
        var duracion = new Entry { Placeholder = "Duración estimada en minutos", Keyboard = Keyboard.Numeric, BackgroundColor = Colors.White, TextColor = Color.FromArgb("#1E293B"), HeightRequest = 48 };
        var save = NegocioFormPage.SaveButton();
        save.Clicked += async (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(nombre.Text) || string.IsNullOrWhiteSpace(descripcion.Text) || !int.TryParse(duracion.Text, out var minutos) || minutos <= 0)
            { await DisplayAlert("Datos inválidos", "Completa los campos y escribe una duración mayor que cero.", "Aceptar"); return; }
            var data = Application.Current!.Handler!.MauiContext!.Services.GetRequiredService<MemoryDataService>();
            data.AddServicio(new Servicio { Nombre = nombre.Text.Trim(), Descripcion = descripcion.Text.Trim(), DuracionMinutos = minutos });
            await DisplayAlert("Guardado", "El servicio se guardó en memoria.", "Aceptar");
            await Shell.Current.GoToAsync("//AppMenu/ServiciosPage");
        };
        Content = NegocioFormPage.FormLayout("Nuevo servicio", "Define un servicio que ofrece el negocio", nombre, descripcion, duracion, save);
=======

using FilaVirtual.ViewModels;

namespace FilaVirtual.Pages;

public class ServicioFormPage : ContentPage, IQueryAttributable
{
    private readonly ServicioViewModel _viewModel;

    public ServicioFormPage(ServicioViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = viewModel;

        BackgroundColor = Color.FromArgb("#F5F7FC");

        var formulario = new VerticalStackLayout
        {
            Spacing = 8
        };

        var titulo = new Label
        {
            FontSize = 25,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#101828")
        };

        titulo.SetBinding(
            Label.TextProperty,
            nameof(ServicioViewModel.TituloFormulario));

        formulario.Children.Add(titulo);

        FormUi.CrearCampo(
            formulario,
            "Nombre del servicio",
            "Ej. Consulta general",
            nameof(viewModel.Nombre),
            nameof(viewModel.NombreError));

        FormUi.CrearCampo(
            formulario,
            "Descripción",
            "Describe el servicio",
            nameof(viewModel.Descripcion),
            nameof(viewModel.DescripcionError));

        FormUi.CrearCampo(
            formulario,
            "Duración en minutos",
            "Ej. 30",
            nameof(viewModel.DuracionMinutos),
            nameof(viewModel.DuracionMinutosError),
            Keyboard.Numeric);

        formulario.Children.Add(
            FormUi.CrearBoton(
                nameof(viewModel.GuardarCommand)));

        var cancelar = new Button
        {
            Text = "Cancelar",
            BackgroundColor = Color.FromArgb("#E5E7EB"),
            TextColor = Color.FromArgb("#344054"),
            CornerRadius = 12,
            HeightRequest = 48
        };

        cancelar.Clicked += OnCancelarClicked;
        formulario.Children.Add(cancelar);

        Content = FormUi.CrearFormulario(
            "Servicio",
            "Define un servicio que ofrece el negocio",
            formulario);
    }

    public async void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        _viewModel.LimpiarFormularioCommand.Execute(null);

        if (query.TryGetValue("id", out var valor) &&
            int.TryParse(valor?.ToString(), out int id))
        {
            await _viewModel.CargarServicioAsync(id);
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
}
