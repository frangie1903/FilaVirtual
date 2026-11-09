<<<<<<< Updated upstream
=======
using SQLite;

>>>>>>> Stashed changes
namespace FilaVirtual.Models;

public class Cliente
{
<<<<<<< Updated upstream
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
}
=======
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;
}
>>>>>>> Stashed changes
