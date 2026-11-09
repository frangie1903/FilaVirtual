<<<<<<< Updated upstream
using FilaVirtual.Services;
=======
using FilaVirtual.Models;
using FilaVirtual.ViewModels;
>>>>>>> Stashed changes
using Microsoft.Extensions.DependencyInjection;

namespace FilaVirtual.Pages;

public class ClientesPage : ContentPage
{
<<<<<<< Updated upstream
    private readonly MemoryDataService _data;
    private readonly CollectionView _list = new();
=======
    private readonly ClienteViewModel _viewModel;
    private readonly CollectionView _lista;
>>>>>>> Stashed changes

    public ClientesPage()
    {
        Title = "Clientes";
<<<<<<< Updated upstream
        _data = Application.Current!.Handler!.MauiContext!.Services.GetRequiredService<MemoryDataService>();
        BackgroundColor = Color.FromArgb("#F5F7FC");
        _list.SelectionMode = SelectionMode.None;
        _list.ItemTemplate = new DataTemplate(() =>
        {
            var label = new Label { FontSize = 15, TextColor = Color.FromArgb("#1E293B") };
            label.SetBinding(Label.TextProperty, ".");
            return new Border { BackgroundColor = Colors.White, Stroke = Color.FromArgb("#E5EAF2"), Padding = 15, Margin = new Thickness(0,0,0,10), StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 }, Content = label };
        });
        var add = new Button { Text = "+  Nuevo cliente", BackgroundColor = Color.FromArgb("#087BF5"), TextColor = Colors.White, CornerRadius = 12, HeightRequest = 50, Margin = new Thickness(20,8,20,18) };
        add.Clicked += async (_, _) => await Shell.Current.GoToAsync("ClienteFormPage");
        Content = new Grid { RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto) }, Children =
        {
            new VerticalStackLayout { Padding = new Thickness(20,24,20,10), Children = { new Label { Text = "Clientes", FontSize = 28, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#101828") }, new Label { Text = "Personas registradas", FontSize = 13, TextColor = Color.FromArgb("#667085") } } },
            _list, add
        }};
        Grid.SetRow(_list, 1); Grid.SetRow(add, 2);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _list.ItemsSource = _data.Clientes.Select(c => $"{c.Nombre}\n{c.Correo} · {c.Telefono}").ToList();
    }
}
=======
        BackgroundColor = Color.FromArgb("#F5F7FC");

        _viewModel = Application.Current!
            .Handler!
            .MauiContext!
            .Services
            .GetRequiredService<ClienteViewModel>();

        BindingContext = _viewModel;

        _lista = new CollectionView
        {
            SelectionMode = SelectionMode.None,
            Margin = new Thickness(20, 10)
        };

        _lista.SetBinding(
            ItemsView.ItemsSourceProperty,
            nameof(ClienteViewModel.Clientes));

        _lista.ItemTemplate = new DataTemplate(() =>
        {
            var nombre = new Label
            {
                FontSize = 17,
                FontAttributes = FontAttributes.Bold
            };
            nombre.SetBinding(Label.TextProperty, nameof(Cliente.Nombre));

            var correo = new Label
            {
                FontSize = 13,
                TextColor = Colors.Gray
            };
            correo.SetBinding(Label.TextProperty, nameof(Cliente.Correo));

            var telefono = new Label
            {
                FontSize = 13,
                TextColor = Colors.Gray
            };
            telefono.SetBinding(Label.TextProperty, nameof(Cliente.Telefono));

            var editar = new Button
            {
                Text = "Editar",
                BackgroundColor = Color.FromArgb("#087BF5"),
                TextColor = Colors.White
            };

            editar.Clicked += async (sender, args) =>
            {
                if (((Button)sender).BindingContext is Cliente cliente)
                {
                    await Shell.Current.GoToAsync(
                        $"ClienteFormPage?id={cliente.Id}");
                }
            };

            var eliminar = new Button
            {
                Text = "Eliminar",
                BackgroundColor = Color.FromArgb("#DC3545"),
                TextColor = Colors.White
            };

            eliminar.Clicked += async (sender, args) =>
            {
                if (((Button)sender).BindingContext is Cliente cliente)
                    await _viewModel.EliminarCommand.ExecuteAsync(cliente);
            };

            return new Border
            {
                Padding = 15,
                Margin = new Thickness(0, 0, 0, 12),
                BackgroundColor = Colors.White,
                Content = new VerticalStackLayout
                {
                    Spacing = 6,
                    Children =
                    {
                        nombre,
                        correo,
                        telefono,
                        new HorizontalStackLayout
                        {
                            Spacing = 10,
                            Children = { editar, eliminar }
                        }
                    }
                }
            };
        });

        var nuevo = new Button
        {
            Text = "+ Nuevo cliente",
            Margin = new Thickness(20),
            BackgroundColor = Color.FromArgb("#087BF5"),
            TextColor = Colors.White
        };

        nuevo.Clicked += async (_, _) =>
            await Shell.Current.GoToAsync("ClienteFormPage");

        Content = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            },
            Children =
            {
                new Label
                {
                    Text = "Clientes registrados",
                    FontSize = 26,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(20, 20, 20, 10)
                },
                _lista,
                nuevo
            }
        };

        Grid.SetRow(_lista, 1);
        Grid.SetRow(nuevo, 2);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarCommand.ExecuteAsync(null);

    }
}
>>>>>>> Stashed changes
