using APPVitalPets.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //private const string BaseUrl = "https://10.0.2.2:7287/api";
        private static string BaseUrl =>
        DeviceInfo.Platform == DevicePlatform.Android
        ? "https://10.0.2.2:7287/api"
        : "https://localhost:7287/api";

        public async Task<Usuario?> LoginAsync(string user, string pass)
        {
            //Si al ejecutar este codigo se devuelve un error a este tipo de lineas verificar que se este ejecutando las APIS y que la URL sea correcta
            var response = await _client.GetAsync($"{BaseUrl}/Usuarios/login?RUsuario={user}&Contrasena={pass}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var usuario = JsonSerializer.Deserialize<Usuario>(json, options);
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
            //Verificamos si es que si traen la informacion correcta del API, mensaje en consola
            Debug.WriteLine($"[DEBUG] JSON recibido desde la API: {content}");
            return JsonSerializer.Deserialize<List<Mascota>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new();
        }

        /*public async Task<bool> CrearMascotaAsync(Mascota mascota)
        {
            var json = JsonSerializer.Serialize(mascota);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseUrl}/Mascotas", content);
            return response.IsSuccessStatusCode;
        }*/
        public async Task<bool> CrearMascotaAsync(Mascota mascota)
        {
            var json = JsonSerializer.Serialize(mascota);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{BaseUrl}/Mascotas", content);
            var cuerpo = await response.Content.ReadAsStringAsync();
            await App.Current.MainPage.DisplayAlert($"Status {response.StatusCode}", cuerpo, "OK");

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ActualizarMascotaAsync(Mascota mascota)
        {
            var json = JsonSerializer.Serialize(mascota);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{BaseUrl}/Mascotas/{mascota.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarMascotaAsync(int id)
        {
            var response = await _client.DeleteAsync($"{BaseUrl}/Mascotas/{id}");
            return response.IsSuccessStatusCode;
        }

        // —— CITAS ——
        public async Task<List<Cita>> ObtenerCitasAsync()
        {
            var resp = await _client.GetAsync($"{BaseUrl}/Citas");
            if (!resp.IsSuccessStatusCode) return new();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Cita>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }

        public async Task<bool> CrearCitaAsync(Cita cita)
        {
            var json = JsonSerializer.Serialize(cita);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _client.PostAsync($"{BaseUrl}/Citas", content);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> ActualizarCitaAsync(Cita cita)
        {
            var json = JsonSerializer.Serialize(cita);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _client.PutAsync($"{BaseUrl}/Citas/{cita.Id}", content);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarCitaAsync(int id)
        {
            var resp = await _client.DeleteAsync($"{BaseUrl}/Citas/{id}");
            return resp.IsSuccessStatusCode;
        }

        // —— VETERINARIOS ——
        public async Task<List<Veterinario>> ObtenerVeterinariosAsync()
        {
            var resp = await _client.GetAsync($"{BaseUrl}/Veterinarios");
            if (!resp.IsSuccessStatusCode) return new();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Veterinario>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }

        public async Task<bool> CrearVeterinarioAsync(Veterinario vet)
        {
            var json = JsonSerializer.Serialize(vet);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _client.PostAsync($"{BaseUrl}/Veterinarios", content);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> ActualizarVeterinarioAsync(Veterinario vet)
        {
            var json = JsonSerializer.Serialize(vet);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _client.PutAsync($"{BaseUrl}/Veterinarios/{vet.Id}", content);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarVeterinarioAsync(int id)
        {
            var resp = await _client.DeleteAsync($"{BaseUrl}/Veterinarios/{id}");
            return resp.IsSuccessStatusCode;
        }

    }
}
