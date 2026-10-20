using FilaVirtual.Models;
using System.Collections.ObjectModel;

namespace FilaVirtual.Services;

// Servicio temporal en memoria. Los registros se pierden al cerrar la app.
public class MemoryDataService
{
    private int _nextTurnoId = 1;
    private int _nextNegocioId = 1;
    private int _nextServicioId = 1;
    private int _nextClienteId = 1;

    public ObservableCollection<Turno> Turnos { get; } = new();
    public ObservableCollection<Negocio> Negocios { get; } = new();
    public ObservableCollection<Servicio> Servicios { get; } = new();
    public ObservableCollection<Cliente> Clientes { get; } = new();

    public MemoryDataService()
    {
        // Datos iniciales para que los listados no aparezcan vacíos en la demostración.
        Negocios.Add(new Negocio { Id = _nextNegocioId++, Nombre = "Clínica Central", Direccion = "Av. Principal", Telefono = "809-555-0101" });
        Servicios.Add(new Servicio { Id = _nextServicioId++, Nombre = "Consulta general", Descripcion = "Atención médica general", DuracionMinutos = 20 });
        Clientes.Add(new Cliente { Id = _nextClienteId++, Nombre = "Cliente de ejemplo", Correo = "cliente@ejemplo.com", Telefono = "809-555-0102" });
        Turnos.Add(new Turno { Id = _nextTurnoId++, Numero = "A-025", Negocio = "Clínica Central", Servicio = "Consulta general", Estado = "En espera", Fecha = DateTime.Today, Hora = new TimeSpan(10, 30, 0), PersonasDelante = 3 });
        Turnos.Add(new Turno { Id = _nextTurnoId++, Numero = "B-018", Negocio = "Banco Popular", Servicio = "Servicio al cliente", Estado = "Completado", Fecha = DateTime.Today, Hora = new TimeSpan(14, 0, 0), PersonasDelante = 0 });
    }

    public void AddTurno(Turno item) { item.Id = _nextTurnoId++; Turnos.Insert(0, item); }
    public void AddNegocio(Negocio item) { item.Id = _nextNegocioId++; Negocios.Insert(0, item); }
    public void AddServicio(Servicio item) { item.Id = _nextServicioId++; Servicios.Insert(0, item); }
    public void AddCliente(Cliente item) { item.Id = _nextClienteId++; Clientes.Insert(0, item); }
}
