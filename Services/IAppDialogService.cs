using System;
using System.Collections.Generic;
using System.Text;

namespace FilaVirtual.Services;

public interface IAppDialogService
{
    Task MostrarExitoAsync(string mensaje);
}