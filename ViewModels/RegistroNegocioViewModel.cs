using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilaVirtual.Data;
using FilaVirtual.Models;
using FilaVirtual.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FilaVirtual.ViewModels;

// Un día de la semana dentro del formulario de una sucursal.
public partial class DiaHorarioEditable : ObservableObject
{
    public DiaHorarioEditable(DayOfWeek dia, string nombre, bool abierto)
    {
        Dia = dia;
        Nombre = nombre;
        this.abierto = abierto;
    }

    public DayOfWeek Dia { get; }
    public string Nombre { get; }

    [ObservableProperty] private bool abierto;
    [ObservableProperty] private TimeSpan apertura = new(8, 0, 0);
    [ObservableProperty] private TimeSpan cierre = new(17, 0, 0);
}

// Fila editable de una sucursal dentro del formulario.
public partial class SucursalEditable : ObservableObject
{
    // 0 = sucursal nueva; > 0 = ya existe en la base de datos.
    public int Id { get; set; }

    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string localidad = string.Empty;
    [ObservableProperty] private string prefijo = "A";

    // ---- Acceso de ESTA sucursal (rol Negocio): opcional ----
    [ObservableProperty] private string operadorCorreo = string.Empty;
    [ObservableProperty] private string operadorContrasena = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OperadorAyuda))]
    private bool operadorExiste;

    public string OperadorAyuda => OperadorExiste
        ? "Esta sucursal ya tiene acceso. Deja la contraseña vacía para no cambiarla."
        : "Opcional: usuario con el que esta sucursal marcará los turnos como atendidos y finalizados.";

    // Lunes a domingo. Por defecto: lunes a viernes abierto, fin de semana cerrado.
    public ObservableCollection<DiaHorarioEditable> Dias { get; } = new()
    {
        new(DayOfWeek.Monday,    "Lunes",     true),
        new(DayOfWeek.Tuesday,   "Martes",    true),
        new(DayOfWeek.Wednesday, "Miércoles", true),
        new(DayOfWeek.Thursday,  "Jueves",    true),
        new(DayOfWeek.Friday,    "Viernes",   true),
        new(DayOfWeek.Saturday,  "Sábado",    false),
        new(DayOfWeek.Sunday,    "Domingo",   false)
    };

    // Copia las horas del primer día abierto a todos los demás días abiertos.
    [RelayCommand]
    private void IgualarHoras()
    {
        var origen = Dias.FirstOrDefault(d => d.Abierto);

        if (origen is null)
            return;

        foreach (var d in Dias.Where(d => d.Abierto))
        {
            d.Apertura = origen.Apertura;
            d.Cierre = origen.Cierre;
        }
    }

    public List<TramoDia> ObtenerTramos() =>
        Dias.Where(d => d.Abierto)
            .Select(d => new TramoDia(d.Dia, d.Apertura, d.Cierre))
            .ToList();

    public void CargarTramos(IEnumerable<TramoDia> tramos)
    {
        var lista = tramos.ToList();

        foreach (var d in Dias)
        {
            var tramo = lista.FirstOrDefault(t => t.Dia == d.Dia);

            d.Abierto = tramo is not null;

            if (tramo is not null)
            {
                d.Apertura = tramo.Apertura;
                d.Cierre = tramo.Cierre;
            }
        }
    }
}

// Fila de la lista de servicios con casilla de selección.
public partial class ServicioSeleccionable : ObservableObject
{
    public ServicioSeleccionable(Servicio servicio) => Servicio = servicio;

    public Servicio Servicio { get; }

    [ObservableProperty] private bool seleccionado;
}

/// <summary>
/// ADMIN: registra o EDITA un negocio completo (datos + sucursales con horario +
/// tipos de diligencia que ofrece) en una sola transacción.
/// </summary>
public partial class RegistroNegocioViewModel : BaseViewModel
{
    private readonly SQLiteDataService _data;
    private readonly IAppDialogService _dialog;

    public ObservableCollection<SucursalEditable> Sucursales { get; } = new();

    public ObservableCollection<ServicioSeleccionable> Servicios { get; } = new();

    [ObservableProperty]
    private int id;

    public bool EstaEditando => Id > 0;

    public string TituloFormulario =>
        EstaEditando ? "Editar negocio" : "Registrar negocio";

    partial void OnIdChanged(int value)
    {
        OnPropertyChanged(nameof(EstaEditando));
        OnPropertyChanged(nameof(TituloFormulario));
    }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "El nombre del negocio es obligatorio.")]
    [StringLength(80, ErrorMessage = "El nombre no puede superar 80 caracteres.")]
    private string nombre = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "La dirección del negocio es obligatoria.")]
    [StringLength(200, ErrorMessage = "La dirección no puede superar 200 caracteres.")]
    private string direccion = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(
        @"^[0-9+\-\s]{7,15}$",
        ErrorMessage = "Introduce un teléfono válido de 7 a 15 caracteres.")]
    private string telefono = string.Empty;

    public string NombreError => ObtenerError(nameof(Nombre));
    public string DireccionError => ObtenerError(nameof(Direccion));
    public string TelefonoError => ObtenerError(nameof(Telefono));

    public RegistroNegocioViewModel(
        SQLiteDataService data,
        IAppDialogService dialog)
    {
        _data = data;
        _dialog = dialog;
    }

    /// <summary>
    /// negocioId = 0 -> formulario de alta. negocioId > 0 -> carga el negocio
    /// con sus sucursales y los servicios que tiene marcados.
    /// </summary>
    public async Task InicializarAsync(int negocioId = 0)
    {
        LimpiarFormulario();

        try
        {
            var asignados = new HashSet<int>();

            if (negocioId > 0)
            {
                var negocio = await _data.ObtenerPorIdAsync<Negocio>(negocioId);

                if (negocio is null)
                {
                    await Shell.Current.DisplayAlert(
                        "Negocio no encontrado",
                        "No se encontró el negocio seleccionado.",
                        "Aceptar");

                    await Shell.Current.GoToAsync("..");
                    return;
                }

                Id = negocio.Id;
                Nombre = negocio.Nombre;
                Direccion = negocio.Direccion;
                Telefono = negocio.Telefono;

                // Un acceso por sucursal: se indexan por SucursalId.
                var usuariosNegocio = await _data.ObtenerDondeAsync<Usuario>(
                    u => u.NegocioId == negocioId);

                var operadores = usuariosNegocio
                    .Where(u => u.Rol == RolUsuario.Negocio && u.SucursalId > 0)
                    .GroupBy(u => u.SucursalId)
                    .ToDictionary(g => g.Key, g => g.First());

                var sucursales = await _data.ObtenerDondeAsync<Sucursal>(
                    s => s.NegocioId == negocioId);

                Sucursales.Clear();

                foreach (var s in sucursales.OrderBy(s => s.Nombre))
                {
                    var fila = new SucursalEditable
                    {
                        Id = s.Id,
                        Nombre = s.Nombre,
                        Localidad = s.Localidad,
                        Prefijo = s.Prefijo
                    };

                    fila.CargarTramos(s.ObtenerTramos());

                    if (operadores.TryGetValue(s.Id, out var operador))
                    {
                        fila.OperadorCorreo = operador.Correo;
                        fila.OperadorExiste = true;
                    }

                    Sucursales.Add(fila);
                }

                if (Sucursales.Count == 0)
                    Sucursales.Add(new SucursalEditable());

                var enlaces = await _data.ObtenerDondeAsync<NegocioServicio>(
                    x => x.NegocioId == negocioId);

                asignados = enlaces.Select(e => e.ServicioId).ToHashSet();
            }

            var servicios = await _data.ObtenerTodosAsync<Servicio>();

            foreach (var s in servicios.OrderBy(s => s.Nombre))
            {
                Servicios.Add(new ServicioSeleccionable(s)
                {
                    Seleccionado = asignados.Contains(s.Id)
                });
            }

            ClearErrors();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error", $"No se pudo cargar el formulario: {ex.Message}", "Aceptar");
        }
    }

    [RelayCommand]
    private void AgregarSucursal() => Sucursales.Add(new SucursalEditable());

    [RelayCommand]
    private void QuitarSucursal(SucursalEditable? sucursal)
    {
        if (sucursal is not null)
            Sucursales.Remove(sucursal);
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        ValidateAllProperties();

        if (HasErrors)
            return;

        string? problema = ValidarSucursales();

        if (problema is not null)
        {
            await Shell.Current.DisplayAlert("Revisa los datos", problema, "Aceptar");
            return;
        }

        try
        {
            // Instantáneas para usarlas dentro de la transacción.
            bool editando = EstaEditando;
            int negocioId = Id;
            string nombreN = Nombre.Trim();
            string direccionN = Direccion.Trim();
            string telefonoN = Telefono.Trim();

            var sucursales = Sucursales.ToList();

            // Se leen en el hilo de UI; dentro de la transacción solo se usan copias.
            var tramosPor = sucursales.ToDictionary(s => s, s => s.ObtenerTramos());

            var idsServicios = Servicios
                .Where(s => s.Seleccionado)
                .Select(s => s.Servicio.Id)
                .ToList();

            // Todo o nada: si falla algo, no queda un negocio "a medias".
            await _data.EjecutarEnTransaccionAsync(db =>
            {
                var negocio = new Negocio
                {
                    Id = negocioId,
                    Nombre = nombreN,
                    Direccion = direccionN,
                    Telefono = telefonoN
                };

                if (editando)
                    db.Update(negocio);
                else
                    db.Insert(negocio); // sqlite-net rellena negocio.Id

                // --- Sucursales: se actualizan, agregan o eliminan SIN recrearlas,
                // para conservar su Id (los contadores y tickets dependen de él).
                var existentes = editando
                    ? db.Table<Sucursal>().Where(x => x.NegocioId == negocioId).ToList()
                    : new List<Sucursal>();

                var idsEnFormulario = sucursales
                    .Where(s => s.Id > 0)
                    .Select(s => s.Id)
                    .ToHashSet();

                foreach (var quitada in existentes.Where(e => !idsEnFormulario.Contains(e.Id)))
                {
                    int quitadaId = quitada.Id;

                    int enEspera = db.Table<Turno>().Count(t =>
                        t.SucursalId == quitadaId &&
                        t.Estado == EstadoTurno.EnEspera);

                    if (enEspera > 0)
                    {
                        throw new InvalidOperationException(
                            $"No puedes quitar la sucursal {quitada.Nombre}: " +
                            $"tiene {enEspera} ticket(s) en espera.");
                    }

                    // El acceso de esa sucursal deja de tener sentido.
                    db.Execute(
                        "DELETE FROM Usuario WHERE SucursalId = ? AND Rol = ?",
                        quitadaId, (int)RolUsuario.Negocio);

                    db.Delete(quitada);
                }

                foreach (var s in sucursales)
                {
                    var tramos = tramosPor[s];

                    var entidad = new Sucursal
                    {
                        Id = s.Id,
                        NegocioId = negocio.Id,
                        Nombre = s.Nombre.Trim(),
                        Localidad = s.Localidad.Trim(),
                        Prefijo = s.Prefijo.Trim().ToUpperInvariant(),
                        HorarioSemanal = HorarioHelper.Serializar(tramos),
                        // Campos antiguos, sincronizados por compatibilidad.
                        HoraApertura = tramos.Min(x => x.Apertura),
                        HoraCierre = tramos.Max(x => x.Cierre),
                        DiasAtencion = string.Join(",", tramos.Select(x => (int)x.Dia))
                    };

                    if (s.Id > 0)
                        db.Update(entidad);
                    else
                        db.Insert(entidad); // rellena entidad.Id

                    // --- Acceso (rol Negocio) de ESTA sucursal: crear o actualizar ---
                    string correoOp = s.OperadorCorreo.Trim().ToLowerInvariant();

                    if (correoOp.Length > 0)
                    {
                        int sid = entidad.Id;
                        string passOp = s.OperadorContrasena;

                        var existente = db.Table<Usuario>()
                            .Where(u => u.SucursalId == sid)
                            .ToList()
                            .FirstOrDefault(u => u.Rol == RolUsuario.Negocio);

                        var otro = db.Table<Usuario>().FirstOrDefault(u => u.Correo == correoOp);

                        if (otro is not null && otro.Id != (existente?.Id ?? 0))
                        {
                            throw new InvalidOperationException(
                                $"El correo {correoOp} ya está en uso por otro usuario.");
                        }

                        if (existente is null)
                        {
                            var nuevo = AuthService.CrearUsuario(
                                $"Operador {nombreN} - {entidad.Nombre}",
                                correoOp, passOp, RolUsuario.Negocio);

                            nuevo.NegocioId = negocio.Id;
                            nuevo.SucursalId = sid;
                            db.Insert(nuevo);
                        }
                        else
                        {
                            existente.Correo = correoOp;
                            existente.NegocioId = negocio.Id;

                            if (passOp.Length > 0)
                                AuthService.AsignarContrasena(existente, passOp);

                            db.Update(existente);
                        }
                    }
                }

                // --- Servicios: nadie referencia los Id de esta tabla,
                // así que se reemplaza el conjunto completo.
                db.Execute("DELETE FROM NegocioServicio WHERE NegocioId = ?", negocio.Id);

                foreach (int servicioId in idsServicios)
                {
                    db.Insert(new NegocioServicio
                    {
                        NegocioId = negocio.Id,
                        ServicioId = servicioId
                    });
                }
            });

            await _dialog.MostrarExitoAsync(editando
                ? "El negocio fue actualizado correctamente."
                : "El negocio fue registrado correctamente.");

            LimpiarFormulario();
            await Shell.Current.GoToAsync("..");
        }
        catch (InvalidOperationException ex)
        {
            await Shell.Current.DisplayAlert("No se pudo guardar", ex.Message, "Aceptar");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error", $"No se pudo guardar el negocio: {ex.Message}", "Aceptar");
        }
    }

    private string? ValidarSucursales()
    {
        if (Sucursales.Count == 0)
            return "Agrega al menos una sucursal.";

        int n = 0;

        foreach (var s in Sucursales)
        {
            n++;

            if (string.IsNullOrWhiteSpace(s.Nombre) || string.IsNullOrWhiteSpace(s.Localidad))
                return $"Sucursal {n}: escribe el nombre y la localidad.";

            if (!Regex.IsMatch(s.Prefijo.Trim(), "^[A-Za-z]{1,3}$"))
                return $"Sucursal {n}: el prefijo del ticket debe tener de 1 a 3 letras.";

            var abiertos = s.Dias.Where(d => d.Abierto).ToList();

            if (abiertos.Count == 0)
                return $"Sucursal {n}: marca al menos un día de atención.";

            var malo = abiertos.FirstOrDefault(d => d.Cierre <= d.Apertura);

            if (malo is not null)
                return $"Sucursal {n}: el {malo.Nombre.ToLowerInvariant()} debe cerrar después de abrir.";

            string? errorAcceso = ValidarAcceso(s, n);

            if (errorAcceso is not null)
                return errorAcceso;
        }

        var correos = Sucursales
            .Select(s => s.OperadorCorreo.Trim().ToLowerInvariant())
            .Where(c => c.Length > 0)
            .ToList();

        if (correos.Count != correos.Distinct().Count())
            return "Dos sucursales tienen el mismo correo de acceso. Cada una necesita el suyo.";

        return null;
    }

    // El acceso de cada sucursal es opcional: correo vacío = no se crea ni se cambia.
    private static string? ValidarAcceso(SucursalEditable s, int n)
    {
        string correo = s.OperadorCorreo.Trim();
        string pass = s.OperadorContrasena;

        if (correo.Length == 0)
            return null;

        if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return $"Sucursal {n}: el correo de acceso no es válido.";

        if (!s.OperadorExiste && pass.Length < 6)
            return $"Sucursal {n}: escribe una contraseña de al menos 6 caracteres para su acceso.";

        if (s.OperadorExiste && pass.Length > 0 && pass.Length < 6)
            return $"Sucursal {n}: la nueva contraseña debe tener al menos 6 caracteres.";

        return null;
    }

    private void LimpiarFormulario()
    {
        Id = 0;
        Nombre = string.Empty;
        Direccion = string.Empty;
        Telefono = string.Empty;

        Sucursales.Clear();
        Servicios.Clear();
        Sucursales.Add(new SucursalEditable());

        ClearErrors();
    }
}
