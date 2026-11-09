using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace FilaVirtual.Models
{
    public class Turno
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Numero { get; set; } = string.Empty;

        public string Negocio { get; set; } = string.Empty;

        public string Servicio { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        public TimeSpan Hora { get; set; }

        public int PersonasDelante { get; set; }

        // ---- Campos nuevos para el sistema de tickets ----
        // Las filas antiguas quedan con 0 / "" y siguen funcionando.

        [Indexed]
        public int UsuarioId { get; set; }

        public int NegocioId { get; set; }

        [Indexed]
        public int SucursalId { get; set; }

        public int ServicioId { get; set; }

        public string Sucursal { get; set; } = string.Empty;

        // Número secuencial del día dentro de la sucursal (1, 2, 3...).
        public int Secuencia { get; set; }

        // Los rellena el operador del negocio.
        public DateTime? AtendidoEn { get; set; }

        public DateTime? FinalizadoEn { get; set; }
    }
}