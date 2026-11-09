<<<<<<< Updated upstream
=======
using SQLite;

>>>>>>> Stashed changes
namespace FilaVirtual.Models;

public class Servicio
{
<<<<<<< Updated upstream
=======
    [PrimaryKey, AutoIncrement]
>>>>>>> Stashed changes
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int DuracionMinutos { get; set; }
}
