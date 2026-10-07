using System;
using System.Collections.Generic;
using System.Text;

namespace FilaVirtual.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnContinuarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("HomePage");
    }
}
