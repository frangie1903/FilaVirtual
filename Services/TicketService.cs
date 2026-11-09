using FilaVirtual.Data;
using FilaVirtual.Models;

namespace FilaVirtual.Services;

public class TicketService
{
    private readonly SQLiteDataService _data;

    public TicketService(SQLiteDataService data)
    {
        _data = data;
    }

    /// <summary>
    /// Crea el ticket con el siguiente número de la sucursal para HOY.
    /// Todo ocurre dentro de UNA transacción: leer contador -> incrementar ->
    /// guardar ticket. Si algo falla, se revierte y no se "quema" ningún número.
    /// </summary>
    public async Task<Turno> GenerarAsync(
        Usuario usuario,
        Negocio negocio,
        Sucursal sucursal,
        Servicio servicio)
    {
        DateTime ahora = DateTime.Now;
        DateTime hoy = ahora.Date;

        if (!sucursal.EstaAbierta(ahora))
        {
            throw new InvalidOperationException(
                $"La sucursal {sucursal.Nombre} está cerrada ahora. " +
                $"Horario: {sucursal.DescribirHorario()}.");
        }

        // Variables locales: sqlite-net traduce la expresión a SQL
        // y funciona mejor con valores simples que con propiedades anidadas.
        int usuarioId = usuario.Id;
        int sucursalId = sucursal.Id;
        string clave = $"{sucursalId}|{hoy:yyyyMMdd}";

        Turno? ticket = null;

        await _data.EjecutarEnTransaccionAsync(db =>
        {
            // Regla de negocio: un solo ticket activo por persona y sucursal.
            var activo = db.Table<Turno>().FirstOrDefault(t =>
                t.UsuarioId == usuarioId &&
                t.SucursalId == sucursalId &&
                t.Fecha == hoy &&
                t.Estado == EstadoTurno.EnEspera);

            if (activo is not null)
            {
                throw new InvalidOperationException(
                    $"Ya tienes el ticket {activo.Numero} en espera en esta sucursal.");
            }

            // 1) Siguiente número.
            var contador = db.Find<ContadorTicket>(clave);

            if (contador is null)
            {
                contador = new ContadorTicket { Clave = clave, Ultimo = 0 };
                db.Insert(contador);
            }

            contador.Ultimo++;
            db.Update(contador);

            // 2) Personas esperando antes que él.
            int delante = db.Table<Turno>().Count(t =>
                t.SucursalId == sucursalId &&
                t.Fecha == hoy &&
                t.Estado == EstadoTurno.EnEspera);

            // 3) Guardar el ticket. Se rellenan también los campos de texto
            //    (Negocio, Servicio) que ya usa tu TurnosPage.
            ticket = new Turno
            {
                Numero = $"{sucursal.Prefijo}-{contador.Ultimo:000}",
                Secuencia = contador.Ultimo,
                Negocio = negocio.Nombre,
                Sucursal = sucursal.Nombre,
                Servicio = servicio.Nombre,
                Estado = EstadoTurno.EnEspera,
                Fecha = hoy,
                Hora = ahora.TimeOfDay,
                PersonasDelante = delante,
                UsuarioId = usuarioId,
                NegocioId = negocio.Id,
                SucursalId = sucursalId,
                ServicioId = servicio.Id
            };

            db.Insert(ticket);
        });

        return ticket!;
    }
}
