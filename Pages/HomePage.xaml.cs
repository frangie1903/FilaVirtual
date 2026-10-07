using System;
using System.Collections.Generic;
using System.Text;

namespace FilaVirtual.Pages;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnMisTurnosClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("TurnosPage");
    }

    private async void OnEscanearQrClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("QrScannerPage");
    }
}