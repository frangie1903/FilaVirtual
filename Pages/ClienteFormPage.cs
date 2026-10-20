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
