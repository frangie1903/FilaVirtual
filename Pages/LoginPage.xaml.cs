namespace FilaVirtual.Pages;

public partial class LoginPage : ContentPage
{
    // Credenciales fijas únicamente para la práctica académica.
    private const string UsuarioValido = "demo@filavirtual.com";
    private const string ContrasenaValida = "Fila123";

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnContinuarClicked(object sender, EventArgs e)
    {
        var usuario = EmailEntry.Text?.Trim() ?? string.Empty;
        var contrasena = PasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
        {
            await DisplayAlert("Campos obligatorios", "Escribe el correo y la contraseña para continuar.", "Aceptar");
            return;
        }

        if (usuario != UsuarioValido || contrasena != ContrasenaValida)
        {
            await DisplayAlert("Acceso denegado", "Las credenciales no son correctas. Verifica los datos e inténtalo de nuevo.", "Aceptar");
            PasswordEntry.Text = string.Empty;
            return;
        }

        if (Shell.Current is AppShell shell)
            await shell.MostrarAplicacionAsync();
    }
}
