using System;
using System.Collections.Generic;
using System.Text;

namespace FilaVirtual.Pages;

public partial class QrScannerPage : ContentPage
{
    public QrScannerPage()
    {
        InitializeComponent();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}