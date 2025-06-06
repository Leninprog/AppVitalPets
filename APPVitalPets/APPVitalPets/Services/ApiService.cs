using APPVitalPets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

/*Se debe tener corriendo el proyecto de las APIS para que el MAUI pueda consumirlas
 Eliminar la base de datos que ya se tiene en el localhost y volver a hacer update por cambios en la variable Usuario*/

namespace APPVitalPets.Services
{
    public class ApiService
    {
        private readonly HttpClient _client = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });

        //private const string BaseUrl = "si se corre en emulador usar 10.0.2.2 en lugar de localhost";
        private const string BaseUrl = "https://localhost:7287/api";

        public async Task<Usuario?> LoginAsync(string user, string pass)
        {
            var response = await _client.GetAsync($"{BaseUrl}/Usuarios/login?RUsuario={user}&Contrasena={pass}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var usuario = JsonSerializer.Deserialize<Usuario>(json);
                return usuario;
            }
            return null;
        }
        public async Task<bool> RegistrarUsuarioAsync(Usuario usuario)
        {
            var json = JsonSerializer.Serialize(usuario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseUrl}/Usuarios", content);

            // Mostrar respuesta por pantalla para depurar
            var responseBody = await response.Content.ReadAsStringAsync();
            await Application.Current.MainPage.DisplayAlert("Respuesta del servidor", responseBody, "OK");

            return response.IsSuccessStatusCode;
        }

        public async Task<List<Mascota>> ObtenerMascotasAsync()
        {
            var response = await _client.GetAsync($"{BaseUrl}/Mascotas");
            if (!response.IsSuccessStatusCode) return new();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Mascota>>(content) ?? new();
        }

        public async Task<bool> CrearMascotaAsync(Mascota mascota)
        {
            var json = JsonSerializer.Serialize(mascota);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseUrl}/Mascotas", content);
            return response.IsSuccessStatusCode;
        }
    }
}
