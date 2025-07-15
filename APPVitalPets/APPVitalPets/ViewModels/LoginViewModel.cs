using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using APPVitalPets.Models;
using APPVitalPets.Services;
using System.Text.Json;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;

namespace APPVitalPets.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiService _api = new();

        [ObservableProperty] private string rUsuario;
        [ObservableProperty] private string contrasena;

        [RelayCommand]
        public async Task Login()
        {
            if (string.IsNullOrWhiteSpace(RUsuario) || string.IsNullOrWhiteSpace(Contrasena))
            {
                await Shell.Current.DisplayAlert("Error", "Ingresa usuario y contraseña.", "OK");
                return;
            }

            var usuario = await _api.LoginAsync(RUsuario, Contrasena);
            if (usuario != null)
            {
                var js = JsonSerializer.Serialize(usuario);
                Preferences.Set("usuarioJson", js);
                await Shell.Current.GoToAsync("//MascotasPage");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Usuario o contraseña incorrectos", "OK");
            }
        }

        [RelayCommand]
        public async Task IrRegistro() => await Shell.Current.GoToAsync("RegistroPage");
    }
}
