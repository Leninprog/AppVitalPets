using SQLite;
using APPVitalPets.Models;

namespace APPVitalPets.Services
{
    public class MascotaDatabase
    {
        private static MascotaDatabase _instance;
        public static MascotaDatabase Instance => _instance ??= new MascotaDatabase();

        private SQLiteAsyncConnection _database;

        private MascotaDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "mascotas.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Mascota>().Wait();
        }

        public Task<List<Mascota>> ObtenerMascotasAsync() =>
            _database.Table<Mascota>().ToListAsync();

        public Task<int> GuardarMascotaAsync(Mascota mascota) =>
            _database.InsertOrReplaceAsync(mascota);

        public Task<int> EliminarMascotaAsync(Mascota mascota) =>
            _database.DeleteAsync(mascota);
    }

}
