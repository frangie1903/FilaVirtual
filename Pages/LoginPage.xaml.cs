<<<<<<< Updated upstream
=======
using FilaVirtual.Services;

>>>>>>> Stashed changes
namespace FilaVirtual.Pages;

public partial class LoginPage : ContentPage
{
<<<<<<< Updated upstream
    // Credenciales fijas únicamente para la práctica académica.
    private const string UsuarioValido = "demo@filavirtual.com";
    private const string ContrasenaValida = "Fila123";

    public LoginPage()
=======
    private readonly AuthService _auth;

    public LoginPage(AuthService auth)
>>>>>>> Stashed changes
    {
        InitializeComponent();

        _auth = auth;
    }

    private async void OnContinuarClicked(object sender, EventArgs e)
    {
<<<<<<< Updated upstream
        var usuario = EmailEntry.Text?.Trim() ?? string.Empty;
        var contrasena = PasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
=======
        var correo = EmailEntry.Text?.Trim() ?? string.Empty;
        var contrasena = PasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
>>>>>>> Stashed changes
        {
            await DisplayAlert("Campos obligatorios", "Escribe el correo y la contraseña para continuar.", "Aceptar");
            return;
        }

<<<<<<< Updated upstream
        if (usuario != UsuarioValido || contrasena != ContrasenaValida)
        {
            await DisplayAlert("Acceso denegado", "Las credenciales no son correctas. Verifica los datos e inténtalo de nuevo.", "Aceptar");
            PasswordEntry.Text = string.Empty;
            return;
        }

        if (Shell.Current is AppShell shell)
            await shell.MostrarAplicacionAsync();
=======
        try
        {
            // Valida contra la tabla Usuario y abre la sesión (con su rol).
            var usuario = await _auth.IniciarSesionAsync(correo, contrasena);

            if (usuario is null)
            {
                await DisplayAlert("Acceso denegado", "Las credenciales no son correctas. Verifica los datos e inténtalo de nuevo.", "Aceptar");
                PasswordEntry.Text = string.Empty;
                return;
            }

            PasswordEntry.Text = string.Empty;

            // AppShell decide qué menú mostrar según el rol de la sesión.
            if (Shell.Current is AppShell shell)
                await shell.MostrarAplicacionAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo iniciar sesión: {ex.Message}", "Aceptar");
        }
>>>>>>> Stashed changes
    }
}
