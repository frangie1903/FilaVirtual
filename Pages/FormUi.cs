using System;
using System.Collections.Generic;
using System.Text;

namespace FilaVirtual.Pages;

internal static class FormUi
{
    public static Entry CrearCampo(
        VerticalStackLayout contenedor,
        string titulo,
        string placeholder,
        string propiedad,
        string propiedadError,
        Keyboard? teclado = null)
    {
        contenedor.Children.Add(new Label
        {
            Text = titulo,
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#1D2939")
        });

        var entry = new Entry
        {
            Placeholder = placeholder,
            BackgroundColor = Colors.White,
            TextColor = Color.FromArgb("#1E293B"),
            PlaceholderColor = Color.FromArgb("#98A2B3"),
            HeightRequest = 48,
            Keyboard = teclado ?? Keyboard.Default
        };

        entry.SetBinding(Entry.TextProperty, propiedad);

        contenedor.Children.Add(entry);

        var error = new Label
        {
            TextColor = Colors.Red,
            FontSize = 11
        };

        error.SetBinding(Label.TextProperty, propiedadError);

        contenedor.Children.Add(error);

        return entry;
    }

    public static Button CrearBoton(string comando)
    {
        var boton = new Button
        {
            Text = "Guardar",
            BackgroundColor = Color.FromArgb("#087BF5"),
            TextColor = Colors.White,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 12,
            HeightRequest = 50
        };

        boton.SetBinding(Button.CommandProperty, comando);

        return boton;
    }

    public static View CrearFormulario(
        string titulo,
        string subtitulo,
        params View[] controles)
    {
        var stack = new VerticalStackLayout
        {
            Padding = 22,
            Spacing = 12
        };

        stack.Children.Add(new Label
        {
            Text = titulo,
            FontSize = 26,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#101828")
        });

        stack.Children.Add(new Label
        {
            Text = subtitulo,
            FontSize = 13,
            TextColor = Color.FromArgb("#667085")
        });

        foreach (var control in controles)
            stack.Children.Add(control);

        return new ScrollView
        {
            Content = stack
        };
    }
}