using FilaVirtual.Models;
using SQLite;
using System.Linq.Expressions;

namespace FilaVirtual.Data;

public class SQLiteDataService
{
    private readonly SQLiteAsyncConnection _database;
    private readonly SemaphoreSlim _inicializacionLock = new(1, 1);

    private bool _inicializada;

    public SQLiteDataService()
    {
        string databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "FilaVirtual.db3");

        _database = new SQLiteAsyncConnection(
            databasePath,
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache);
    }

    public async Task InitializeAsync()
    {
        if (_inicializada)
            return;

        await _inicializacionLock.WaitAsync();

        try
        {
            if (_inicializada)
                return;

            await _database.CreateTableAsync<Cliente>();
            await _database.CreateTableAsync<Negocio>();
            await _database.CreateTableAsync<Servicio>();
            await _database.CreateTableAsync<Turno>();
            await _database.CreateTableAsync<Usuario>();
            await _database.CreateTableAsync<Sucursal>();
            await _database.CreateTableAsync<NegocioServicio>();
            await _database.CreateTableAsync<ContadorTicket>();

            _inicializada = true;
        }
        finally
        {
            _inicializacionLock.Release();
        }
    }

    public async Task<List<T>> ObtenerTodosAsync<T>()
        where T : new()
    {
        await InitializeAsync();

        return await _database
            .Table<T>()
            .ToListAsync();
    }

    public async Task<List<T>> ObtenerDondeAsync<T>(
        Expression<Func<T, bool>> filtro)
        where T : new()
    {
        await InitializeAsync();

        return await _database
            .Table<T>()
            .Where(filtro)
            .ToListAsync();
    }

    // Ejecuta varias operaciones como una sola unidad: o se guardan todas
    // o no se guarda ninguna. Dentro de la acción se usa la conexión síncrona.
    public async Task EjecutarEnTransaccionAsync(
        Action<SQLiteConnection> accion)
    {
        ArgumentNullException.ThrowIfNull(accion);

        await InitializeAsync();

        await _database.RunInTransactionAsync(accion);
    }

    public async Task<T?> ObtenerPorIdAsync<T>(int id)
        where T : new()
    {
        await InitializeAsync();

        return await _database.FindAsync<T>(id);
    }

    public async Task<int> InsertarAsync<T>(T entidad)
        where T : new()
    {
        ArgumentNullException.ThrowIfNull(entidad);

        await InitializeAsync();

        return await _database.InsertAsync(entidad);
    }

    public async Task<int> ActualizarAsync<T>(T entidad)
        where T : new()
    {
        ArgumentNullException.ThrowIfNull(entidad);

        await InitializeAsync();

        return await _database.UpdateAsync(entidad);
    }

    public async Task<int> EliminarAsync<T>(T entidad)
        where T : new()
    {
        ArgumentNullException.ThrowIfNull(entidad);

        await InitializeAsync();

        return await _database.DeleteAsync(entidad);
    }

    public async Task<int> EliminarPorIdAsync<T>(int id)
        where T : new()
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(id),
                "El ID debe ser mayor que cero.");
        }

        await InitializeAsync();

        return await _database.DeleteAsync<T>(id);
    }
}