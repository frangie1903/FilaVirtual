using FilaVirtual.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FilaVirtual.Pages;

public class NegociosPage : ContentPage
{
    private readonly MemoryDataService _data;
    private readonly CollectionView _list = new();

    public NegociosPage()
    {
        Title = "Negocios";
        _data = Application.Current!.Handler!.MauiContext!.Services.GetRequiredService<MemoryDataService>();
        BackgroundColor = Color.FromArgb("#F5F7FC");
        _list.SelectionMode = SelectionMode.None;
        _list.ItemTemplate = new DataTemplate(() =>
        {
            var label = new Label { FontSize = 15, TextColor = Color.FromArgb("#1E293B"), VerticalOptions = LayoutOptions.Center };
            label.SetBinding(Label.TextProperty, ".");
            var card = new Border { BackgroundColor = Colors.White, Stroke = Color.FromArgb("#E5EAF2"), Padding = 15, Margin = new Thickness(0,0,0,10), StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 }, Content = label };
            return card;
        });
        var add = new Button { Text = "+  Nuevo negocio", BackgroundColor = Color.FromArgb("#087BF5"), TextColor = Colors.White, CornerRadius = 12, HeightRequest = 50, Margin = new Thickness(20,8,20,18) };
        add.Clicked += async (_, _) => await Shell.Current.GoToAsync("NegocioFormPage");
        Content = new Grid { RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto) }, Children =
        {
            new VerticalStackLayout { Padding = new Thickness(20,24,20,10), Children = { new Label { Text = "Negocios", FontSize = 28, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#101828") }, new Label { Text = "Establecimientos registrados", FontSize = 13, TextColor = Color.FromArgb("#667085") } } },
            _list,
            add
        }};
        Grid.SetRow(_list, 1);
        Grid.SetRow(add, 2);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _list.ItemsSource = _data.Negocios.Select(n => $"{n.Nombre}\n{n.Direccion} · {n.Telefono}").ToList();
    }
}
