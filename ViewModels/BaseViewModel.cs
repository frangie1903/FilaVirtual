
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Linq;

namespace FilaVirtual.ViewModels;

public abstract class BaseViewModel : ObservableValidator
{
    protected BaseViewModel()
    {
        ErrorsChanged += CuandoCambianLosErrores;
    }

    protected string ObtenerError(string propiedad)
    {
        return GetErrors(propiedad)
            .OfType<System.ComponentModel.DataAnnotations.ValidationResult>()
            .Select(error => error.ErrorMessage)
            .FirstOrDefault(mensaje =>
                !string.IsNullOrWhiteSpace(mensaje))
            ?? string.Empty;
    }

    private void CuandoCambianLosErrores(
        object? sender,
        DataErrorsChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.PropertyName))
            OnPropertyChanged($"{e.PropertyName}Error");
    }
}
