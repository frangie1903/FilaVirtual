using SQLite;

namespace FilaVirtual.Models;

// Tabla de relación: qué "tipos de diligencia" ofrece cada negocio.
// Se hace así (y no con un NegocioId dentro de Servicio) para NO tocar
// ServicioViewModel: su UpdateAsync reescribiría esa columna al editar.
public class NegocioServicio
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int NegocioId { get; set; }

    [Indexed]
    public int ServicioId { get; set; }
}
