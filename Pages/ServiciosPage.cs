<<<<<<< Updated upstream
using FilaVirtual.Services;
=======
using FilaVirtual.Models;
using FilaVirtual.ViewModels;
>>>>>>> Stashed changes
using Microsoft.Extensions.DependencyInjection;

namespace FilaVirtual.Pages;

public class ServiciosPage : ContentPage
{
<<<<<<< Updated upstream
    private readonly MemoryDataService _data;
    private readonly CollectionView _list = new();
=======
    private readonly ServicioViewModel _viewModel;
    private readonly CollectionView _lista;
>>>>>>> Stashed changes

    public ServiciosPage()
    {
        Title = "Servicios";
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
        var add = new Button { Text = "+  Nuevo servicio", BackgroundColor = Color.FromArgb("#087BF5"), TextColor = Colors.White, CornerRadius = 12, HeightRequest = 50, Margin = new Thickness(20,8,20,18) };
        add.Clicked += async (_, _) => await Shell.Current.GoToAsync("ServicioFormPage");
        Content = new Grid { RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto) }, Children =
        {
            new VerticalStackLayout { Padding = new Thickness(20,24,20,10), Children = { new Label { Text = "Servicios", FontSize = 28, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#101828") }, new Label { Text = "Servicios disponibles", FontSize = 13, TextColor = Color.FromArgb("#667085") } } },
            _list, add
        }};
        Grid.SetRow(_list, 1); Grid.SetRow(add, 2);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _list.ItemsSource = _data.Servicios.Select(s => $"{s.Nombre}\n{s.Descripcion} · {s.DuracionMinutos} minutos").ToList();
    }
}
=======
        BackgroundColor = Color.FromArgb("#F5F7FC");

        _viewModel = Application.Current!
            .Handler!
            .MauiContext!
            .Services
            .GetRequiredService<ServicioViewModel>();

        BindingContext = _viewModel;

        _lista = new CollectionView
        {
            SelectionMode = SelectionMode.None,
            Margin = new Thickness(20, 5, 20, 10),
            EmptyView = CrearVistaVacia()
        };

        _lista.SetBinding(
            ItemsView.ItemsSourceProperty,
            nameof(ServicioViewModel.Servicios));

        _lista.ItemTemplate = CrearPlantillaServicio();

        var encabezado = new VerticalStackLayout
        {
            Padding = new Thickness(20, 24, 20, 10),
            Spacing = 3,
            Children =
            {
                new Label
                {
                    Text = "Servicios",
                    FontSize = 28,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#101828")
                },
                new Label
                {
                    Text = "Servicios disponibles",
                    FontSize = 13,
                    TextColor = Color.FromArgb("#667085")
                }
            }
        };

        var botonNuevo = new Button
        {
            Text = "+ Nuevo servicio",
            BackgroundColor = Color.FromArgb("#087BF5"),
            TextColor = Colors.White,
            CornerRadius = 12,
            HeightRequest = 50,
            Margin = new Thickness(20, 8, 20, 18)
        };

        botonNuevo.Clicked += OnNuevoServicioClicked;

        var contenedor = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            }
        };

        contenedor.Add(encabezado);
        contenedor.Add(_lista, 0, 1);
        contenedor.Add(botonNuevo, 0, 2);

        Content = contenedor;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.CargarCommand.ExecuteAsync(null);
    }

    private DataTemplate CrearPlantillaServicio()
    {
        return new DataTemplate(() =>
        {
            var nombre = new Label
            {
                FontSize = 17,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#1E293B")
            };

            nombre.SetBinding(
                Label.TextProperty,
                nameof(Servicio.Nombre));

            var descripcion = new Label
            {
                FontSize = 14,
                TextColor = Color.FromArgb("#667085"),
                LineBreakMode = LineBreakMode.WordWrap
            };

            descripcion.SetBinding(
                Label.TextProperty,
                nameof(Servicio.Descripcion));

            var duracion = new Label
            {
                FontSize = 13,
                TextColor = Color.FromArgb("#087BF5"),
                FontAttributes = FontAttributes.Bold
            };

            duracion.SetBinding(
                Label.TextProperty,
                new Binding(
                    nameof(Servicio.DuracionMinutos),
                    stringFormat: "{0} minutos"));

            var botonEditar = new Button
            {
                Text = "Editar",
                FontSize = 13,
                HeightRequest = 40,
                CornerRadius = 10,
                BackgroundColor = Color.FromArgb("#087BF5"),
                TextColor = Colors.White
            };

            botonEditar.Clicked += OnEditarServicioClicked;

            var botonEliminar = new Button
            {
                Text = "Eliminar",
                FontSize = 13,
                HeightRequest = 40,
                CornerRadius = 10,
                BackgroundColor = Color.FromArgb("#DC3545"),
                TextColor = Colors.White
            };

            botonEliminar.Clicked += OnEliminarServicioClicked;

            var acciones = new Grid
            {
                Margin = new Thickness(0, 8, 0, 0),
                ColumnSpacing = 10,
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star)
                }
            };

            acciones.Add(botonEditar, 0, 0);
            acciones.Add(botonEliminar, 1, 0);

            return new Border
            {
                BackgroundColor = Colors.White,
                Stroke = Color.FromArgb("#E5EAF2"),
                StrokeThickness = 1,
                Padding = 15,
                Margin = new Thickness(0, 0, 0, 12),
                StrokeShape =
                    new Microsoft.Maui.Controls.Shapes.RoundRectangle
                    {
                        CornerRadius = 12
                    },
                Content = new VerticalStackLayout
                {
                    Spacing = 5,
                    Children =
                    {
                        nombre,
                        descripcion,
                        duracion,
                        acciones
                    }
                }
            };
        });
    }

    private View CrearVistaVacia()
    {
        return new VerticalStackLayout
        {
            Padding = new Thickness(30),
            Spacing = 8,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Label
                {
                    Text = "No hay servicios registrados",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#344054"),
                    HorizontalTextAlignment =
                        TextAlignment.Center
                },
                new Label
                {
                    Text = "Presiona “Nuevo servicio” para agregar el primero.",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#667085"),
                    HorizontalTextAlignment =
                        TextAlignment.Center
                }
            }
        };
    }

    private async void OnNuevoServicioClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(ServicioFormPage));
    }

    private async void OnEditarServicioClicked(
        object? sender,
        EventArgs e)
    {
        if (sender is not Button boton ||
            boton.BindingContext is not Servicio servicio)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(ServicioFormPage)}?id={servicio.Id}");
    }

    private async void OnEliminarServicioClicked(
        object? sender,
        EventArgs e)
    {
        if (sender is not Button boton ||
            boton.BindingContext is not Servicio servicio)
        {
            return;
        }

        await _viewModel
            .EliminarCommand
            .ExecuteAsync(servicio);
    }
}
>>>>>>> Stashed changes
