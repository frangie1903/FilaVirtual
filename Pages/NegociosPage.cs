<<<<<<< Updated upstream
using FilaVirtual.Services;
=======

using FilaVirtual.Models;
using FilaVirtual.ViewModels;
>>>>>>> Stashed changes
using Microsoft.Extensions.DependencyInjection;

namespace FilaVirtual.Pages;

public class NegociosPage : ContentPage
{
<<<<<<< Updated upstream
    private readonly MemoryDataService _data;
    private readonly CollectionView _list = new();
=======
    private readonly NegocioViewModel _viewModel;
    private readonly CollectionView _lista;
>>>>>>> Stashed changes

    public NegociosPage()
    {
        Title = "Negocios";
<<<<<<< Updated upstream
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
=======
        BackgroundColor = Color.FromArgb("#F5F7FC");

        _viewModel = Application.Current!
            .Handler!
            .MauiContext!
            .Services
            .GetRequiredService<NegocioViewModel>();

        BindingContext = _viewModel;

        _lista = new CollectionView
        {
            SelectionMode = SelectionMode.None,
            Margin = new Thickness(20, 10),
            EmptyView = new Label
            {
                Text = "No hay negocios registrados.",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Colors.Gray
            }
        };

        _lista.SetBinding(
            ItemsView.ItemsSourceProperty,
            nameof(NegocioViewModel.Negocios));

        _lista.ItemTemplate = new DataTemplate(() =>
        {
            var nombre = new Label
            {
                FontSize = 17,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#1E293B")
            };
            nombre.SetBinding(
                Label.TextProperty,
                nameof(Negocio.Nombre));

            var direccion = new Label
            {
                FontSize = 14,
                TextColor = Colors.Gray
            };
            direccion.SetBinding(
                Label.TextProperty,
                nameof(Negocio.Direccion));

            var telefono = new Label
            {
                FontSize = 14,
                TextColor = Colors.Gray
            };
            telefono.SetBinding(
                Label.TextProperty,
                nameof(Negocio.Telefono));

            var editar = new Button
            {
                Text = "Editar",
                BackgroundColor = Color.FromArgb("#087BF5"),
                TextColor = Colors.White
            };

            editar.Clicked += OnEditarClicked;

            var eliminar = new Button
            {
                Text = "Eliminar",
                BackgroundColor = Color.FromArgb("#DC3545"),
                TextColor = Colors.White
            };

            eliminar.Clicked += OnEliminarClicked;

            return new Border
            {
                Padding = 15,
                Margin = new Thickness(0, 0, 0, 12),
                BackgroundColor = Colors.White,
                Stroke = Color.FromArgb("#E5EAF2"),
                StrokeShape =
                    new Microsoft.Maui.Controls.Shapes.RoundRectangle
                    {
                        CornerRadius = 12
                    },
                Content = new VerticalStackLayout
                {
                    Spacing = 6,
                    Children =
                    {
                        nombre,
                        direccion,
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
            Text = "+ Nuevo negocio",
            BackgroundColor = Color.FromArgb("#087BF5"),
            TextColor = Colors.White,
            CornerRadius = 12,
            HeightRequest = 50,
            Margin = new Thickness(20)
        };

        nuevo.Clicked += OnNuevoClicked;

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
                    Text = "Negocios registrados",
                    FontSize = 26,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#101828"),
                    Margin = new Thickness(20)
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

    private async void OnNuevoClicked(
        object? sender,
        EventArgs e)
    {
        // Alta completa: datos + sucursales + horarios + servicios.
        // (NegocioFormPage se sigue usando para EDITAR los datos básicos.)
        await Shell.Current.GoToAsync(
            nameof(RegistroNegocioPage));
    }

    private async void OnEditarClicked(
        object? sender,
        EventArgs e)
    {
        if (sender is Button boton &&
            boton.BindingContext is Negocio negocio)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(RegistroNegocioPage)}?id={negocio.Id}");
        }
    }

    private async void OnEliminarClicked(
        object? sender,
        EventArgs e)
    {
        if (sender is Button boton &&
            boton.BindingContext is Negocio negocio)
        {
            await _viewModel.EliminarCommand.ExecuteAsync(negocio);
        }
>>>>>>> Stashed changes
    }
}
