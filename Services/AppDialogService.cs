using System;
using System.Collections.Generic;
using System.Text;

namespace FilaVirtual.Services;

public class AppDialogService : IAppDialogService
{
    public async Task MostrarExitoAsync(string mensaje)
    {
        var page = Shell.Current?.CurrentPage;

        if (page != null)
        {
            await page.DisplayAlert("FilaVirtual", mensaje, "Aceptar");
        }
    }
}