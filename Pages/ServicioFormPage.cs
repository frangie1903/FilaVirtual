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
    }
}
