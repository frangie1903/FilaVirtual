using SQLite;

namespace FilaVirtual.Models;

// Un registro por sucursal y por día. Clave: "{sucursalId}|{yyyyMMdd}".
// Al cambiar el día la clave cambia, así que la numeración reinicia sola.
public class ContadorTicket
{
    [PrimaryKey]
    public string Clave { get; set; } = string.Empty;

    public int Ultimo { get; set; }
}
