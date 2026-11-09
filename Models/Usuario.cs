using SQLite;

namespace FilaVirtual.Models;

// Cliente = 0 a propósito: si un valor falta o se corrompe, el usuario
// cae en el rol con MENOS privilegios.
public enum RolUsuario
{
    Cliente = 0,
    Admin = 1,
    Negocio = 2   // operador de un negocio: atiende y finaliza turnos
}

public class Usuario
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed(Unique = true)]
    public string Correo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    // Nunca se guarda la contraseña: solo hash PBKDF2 + salt.
    public string PasswordHash { get; set; } = string.Empty;

    public string Salt { get; set; } = string.Empty;

    public RolUsuario Rol { get; set; } = RolUsuario.Cliente;

    // Solo para el rol Negocio: a qué negocio y a qué SUCURSAL pertenece (0 = ninguno).
    // SucursalId = 0 con NegocioId > 0 es un acceso antiguo "de todo el negocio".
    [Indexed]
    public int NegocioId { get; set; }

    [Indexed]
    public int SucursalId { get; set; }
}
