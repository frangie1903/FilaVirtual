using FilaVirtual.Models;
using FilaVirtual.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FilaVirtual.Pages;

public class NegocioFormPage : ContentPage
{
    public NegocioFormPage()
    {
        Title = "Nuevo negocio";
        BackgroundColor = Color.FromArgb("#F5F7FC");
        var nombre = Field("Nombre del negocio", "Ej. Clínica Central");
        var direccion = Field("Dirección", "Dirección del establecimiento");
        var telefono = Field("Teléfono", "809-555-0100");
        var save = SaveButton();
        save.Clicked += async (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(nombre.Text) || string.IsNullOrWhiteSpace(direccion.Text) || string.IsNullOrWhiteSpace(telefono.Text))
            { await DisplayAlert("Campos obligatorios", "Completa nombre, dirección y teléfono.", "Aceptar"); return; }
            var data = Application.Current!.Handler!.MauiContext!.Services.GetRequiredService<MemoryDataService>();
            data.AddNegocio(new Negocio { Nombre = nombre.Text.Trim(), Direccion = direccion.Text.Trim(), Telefono = telefono.Text.Trim() });
            await DisplayAlert("Guardado", "El negocio se guardó en memoria.", "Aceptar");
            await Shell.Current.GoToAsync("//AppMenu/NegociosPage");
        };
        Content = FormLayout("Nuevo negocio", "Registra un establecimiento", nombre, direccion, telefono, save);
    }

    internal static Entry Field(string title, string placeholder) => new() { Placeholder = $"{title} · {placeholder}", BackgroundColor = Colors.White, TextColor = Color.FromArgb("#1E293B"), HeightRequest = 48 };
    internal static Button SaveButton() => new() { Text = "Guardar", BackgroundColor = Color.FromArgb("#087BF5"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, CornerRadius = 12, HeightRequest = 50 };
    internal static View FormLayout(string title, string subtitle, params View[] controls)
    {
        var stack = new VerticalStackLayout { Padding = 22, Spacing = 12 };
        stack.Children.Add(new Label { Text = title, FontSize = 26, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#101828") });
        stack.Children.Add(new Label { Text = subtitle, FontSize = 13, TextColor = Color.FromArgb("#667085") });
        foreach (var c in controls) stack.Children.Add(c);
        return new ScrollView { Content = stack };
    }
}
