using FilaVirtual.Models;

namespace FilaVirtual.Services;

public interface ISesionService
{
    Usuario? UsuarioActual { get; }
    bool EstaAutenticado { get; }
    bool EsAdmin { get; }

    void Iniciar(Usuario usuario);
    void Cerrar();
}
