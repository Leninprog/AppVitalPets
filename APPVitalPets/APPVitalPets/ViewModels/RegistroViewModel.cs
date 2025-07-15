using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using APPVitalPets.Models;
using APPVitalPets.Services;
using System.Threading.Tasks;

namespace APPVitalPets.ViewModels
{
    public partial class RegistroViewModel : ObservableObject
    {
        private readonly ApiService _api = new();

        [ObservableProperty] string rUsuario;
        [ObservableProperty] string contrasena;
        [ObservableProperty] string correo;
        [ObservableProperty] string telefono;
        [ObservableProperty] string direccion;

        [RelayCommand]
        public async Task Registrar()
        {
            var nuevo = new Usuario
            {
                RUsuario = RUsuario,
                Contrasena = Contrasena,
                Correo = Correo,
                Telefono = Telefono,
                Direccion = Direccion
            };

            if (await _api.RegistrarUsuarioAsync(nuevo))
                await Shell.Current.DisplayAlert("OK", "Usuario registrado correctamente", "Cerrar");
            else
                await Shell.Current.DisplayAlert("Error", "No se pudo registrar", "OK");
        }
    }
}
