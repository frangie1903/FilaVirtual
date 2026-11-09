using SQLite;

namespace FilaVirtual.Models;

public class Sucursal
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int NegocioId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    // Localidad / dirección de esta sucursal.
    public string Localidad { get; set; } = string.Empty;

    // Letra(s) con las que empieza el ticket: "A" -> A-001, A-002...
    public string Prefijo { get; set; } = "A";

    // ---- Horario personalizado por día (formato de HorarioHelper) ----
    public string HorarioSemanal { get; set; } = string.Empty;

    // ---- Campos antiguos: se conservan para sucursales creadas antes del
    // horario por día. Si HorarioSemanal está vacío se usan estos. ----
    public TimeSpan HoraApertura { get; set; } = new(8, 0, 0);

    public TimeSpan HoraCierre { get; set; } = new(17, 0, 0);

    public string DiasAtencion { get; set; } = "1,2,3,4,5";

    // OJO: son métodos y no propiedades a propósito. sqlite-net intenta mapear
    // cualquier propiedad pública a una columna (salvo que lleve [Ignore]).
    public List<TramoDia> ObtenerTramos()
    {
        var tramos = HorarioHelper.Leer(HorarioSemanal);

        if (tramos.Count > 0)
            return tramos;

        // Compatibilidad con el formato anterior (mismas horas todos los días).
        return DiasAtencion
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(d => int.TryParse(d, out int n) ? n : -1)
            .Where(n => n is >= 0 and <= 6)
            .Select(n => new TramoDia((DayOfWeek)n, HoraApertura, HoraCierre))
            .ToList();
    }

    public bool EstaAbierta(DateTime ahora) =>
        ObtenerTramos().Any(t =>
            t.Dia == ahora.DayOfWeek &&
            ahora.TimeOfDay >= t.Apertura &&
            ahora.TimeOfDay < t.Cierre);

    public string DescribirHorario() => HorarioHelper.Describir(ObtenerTramos());
}
