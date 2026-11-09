using FilaVirtual.Data;
using FilaVirtual.Models;
using System.Security.Cryptography;

namespace FilaVirtual.Services;

public class AuthService
{
    private const int Iteraciones = 100_000;

    private readonly SQLiteDataService _data;
    private readonly ISesionService _sesion;
    private bool _sembrado;

    public AuthService(SQLiteDataService data, ISesionService sesion)
    {
        _data = data;
        _sesion = sesion;
    }

    /// <summary>
    /// Devuelve el usuario y abre la sesión, o null si las credenciales no son válidas.
    /// </summary>
    public async Task<Usuario?> IniciarSesionAsync(string correo, string contrasena)
    {
        await _data.InitializeAsync();
        await SembrarUsuariosInicialesAsync();

        string correoNormalizado = correo.Trim().ToLowerInvariant();

        var usuarios = await _data.ObtenerDondeAsync<Usuario>(
            u => u.Correo == correoNormalizado);

        var usuario = usuarios.FirstOrDefault();

        if (usuario is null || !VerificarContrasena(contrasena, usuario))
            return null;

        _sesion.Iniciar(usuario);
        return usuario;
    }

    /// <summary>
    /// Registro público: SIEMPRE crea un Cliente. Un Admin solo se crea
    /// desde la siembra inicial o directamente en la base de datos.
    /// </summary>
    public async Task<Usuario> RegistrarClienteAsync(
        string nombre, string correo, string contrasena)
    {
        await _data.InitializeAsync();

        var usuario = CrearUsuario(nombre, correo, contrasena, RolUsuario.Cliente);
        await _data.InsertarAsync(usuario);
        return usuario;
    }

    // Credenciales de DEMOSTRACIÓN. Cámbialas o elimínalas antes de publicar.
    // "demo@filavirtual.com / Fila123" es tu usuario de siempre: ahora es Cliente.
    private async Task SembrarUsuariosInicialesAsync()
    {
        if (_sembrado)
            return;

        var existentes = await _data.ObtenerTodosAsync<Usuario>();

        if (existentes.Count == 0)
        {
            await _data.InsertarAsync(CrearUsuario(
                "Administrador", "admin@filavirtual.com", "Admin123", RolUsuario.Admin));

            await _data.InsertarAsync(CrearUsuario(
                "Cliente demo", "demo@filavirtual.com", "Fila123", RolUsuario.Cliente));
        }

        _sembrado = true;
    }

    public static Usuario CrearUsuario(
        string nombre, string correo, string contrasena, RolUsuario rol)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        return new Usuario
        {
            Nombre = nombre.Trim(),
            Correo = correo.Trim().ToLowerInvariant(),
            Salt = Convert.ToBase64String(salt),
            PasswordHash = CalcularHash(contrasena, salt),
            Rol = rol
        };
    }

    public static void AsignarContrasena(Usuario usuario, string contrasena)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        usuario.Salt = Convert.ToBase64String(salt);
        usuario.PasswordHash = CalcularHash(contrasena, salt);
    }

    private static string CalcularHash(string contrasena, byte[] salt) =>
        Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(
            contrasena, salt, Iteraciones, HashAlgorithmName.SHA256, 32));

    private static bool VerificarContrasena(string contrasena, Usuario usuario)
    {
        byte[] salt = Convert.FromBase64String(usuario.Salt);
        byte[] esperado = Convert.FromBase64String(usuario.PasswordHash);
        byte[] actual = Convert.FromBase64String(CalcularHash(contrasena, salt));

        return CryptographicOperations.FixedTimeEquals(esperado, actual);
    }
}
