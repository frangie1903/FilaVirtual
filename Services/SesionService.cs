using FilaVirtual.Models;

namespace FilaVirtual.Services;

// Singleton: guarda quién está logueado mientras la app está abierta.
public class SesionService : ISesionService
{
    public Usuario? UsuarioActual { get; private set; }

    public bool EstaAutenticado => UsuarioActual is not null;

    public bool EsAdmin => UsuarioActual?.Rol == RolUsuario.Admin;

    public void Iniciar(Usuario usuario) => UsuarioActual = usuario;

    public void Cerrar() => UsuarioActual = null;
}
