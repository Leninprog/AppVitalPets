using System.Text.Json;
using APPVitalPets.Models;

namespace APPVitalPets.Services
{
    public class UsuarioDataService
    {
        private static string BasePath => Path.Combine(FileSystem.AppDataDirectory, "usuarios");

        public UsuarioDataService()
        {
            if (!Directory.Exists(BasePath))
                Directory.CreateDirectory(BasePath);
        }

        public async Task GuardarUsuarioConMascotasAsync(Usuario usuario, List<Mascota> mascotas)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] Entrando a GuardarUsuarioConMascotasAsync");

                var basePath = Path.Combine(FileSystem.AppDataDirectory, "usuarios");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] basePath = {basePath}");

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                    System.Diagnostics.Debug.WriteLine("[DEBUG] Carpeta creada");
                }

                var rutaArchivo = Path.Combine(basePath, $"{usuario.RUsuario}.json");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] rutaArchivo = {rutaArchivo}");

                var datos = new
                {
                    usuario.Id,
                    usuario.RUsuario,
                    usuario.Contrasena,
                    usuario.Correo,
                    usuario.Telefono,
                    usuario.Direccion,
                    Mascotas = mascotas
                };

                var json = JsonSerializer.Serialize(datos, new JsonSerializerOptions { WriteIndented = true });

                await File.WriteAllTextAsync(rutaArchivo, json);

                System.Diagnostics.Debug.WriteLine($"[GUARDADO EN]: {rutaArchivo}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] No se pudo guardar el archivo: {ex.Message}");
            }
        }

        public async Task<string> LeerDatosUsuarioAsync(string cedula)
        {
            var rutaArchivo = Path.Combine(BasePath, $"{cedula}.json");
            return File.Exists(rutaArchivo)
                ? await File.ReadAllTextAsync(rutaArchivo)
                : "No hay datos guardados.";
        }

        public string[] ObtenerArchivosUsuarios()
        {
            return Directory.GetFiles(BasePath, "*.json");
        }
    }
}
