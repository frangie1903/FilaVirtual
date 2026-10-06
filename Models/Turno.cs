using System;
using System.Collections.Generic;
using System.Text;

namespace FilaVirtual.Models
{
    public class Turno
    {
        public int Id { get; set; }

        public string Numero { get; set; } = string.Empty;

        public string Negocio { get; set; } = string.Empty;

        public string Servicio { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        public TimeSpan Hora { get; set; }

        public int PersonasDelante { get; set; }
    }
}