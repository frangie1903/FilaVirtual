using System;
using System.Collections.Generic;
using System.Text;

namespace FilaVirtual.Pages;

public partial class TurnosPage : ContentPage
{
    public TurnosPage()
    {
        InitializeComponent();
    }

    private async void OnNuevoTurnoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("TurnoFormPage");
    }
}
