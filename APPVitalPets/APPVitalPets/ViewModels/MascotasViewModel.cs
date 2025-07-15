using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using APPVitalPets.Models;
using APPVitalPets.Services;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace APPVitalPets.ViewModels;

public partial class MascotasViewModel : ObservableObject
{
    private readonly ApiService _api = new();
    private Usuario usuario;

    [ObservableProperty] private List<Mascota> mascotas = new();
    [ObservableProperty] private string nombre, raza, especie;
    [ObservableProperty] private DateTime fechaNacimiento = DateTime.Today;
    [ObservableProperty] private bool mostrarFormulario;
    [ObservableProperty] private bool editando;

    public MascotasViewModel()
    {
        _ = CargarMascotas(); // <-- esto carga el usuario al iniciar
    }

    [RelayCommand]
    public void MostrarFormularioMascota()
    {
        MostrarFormulario = true;
        Editando = false;
        Nombre = Raza = Especie = string.Empty;
        FechaNacimiento = DateTime.Today;
    }

    [RelayCommand]
    public async Task CargarMascotas()
    {
        var js = Preferences.Get("usuarioJson", string.Empty);
        if (string.IsNullOrEmpty(js))
        {
            await Shell.Current.DisplayAlert("Error", "Sesión no iniciada", "OK");
            return;
        }

        usuario = JsonSerializer.Deserialize<Usuario>(js);
        if (usuario == null)
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo cargar el usuario", "OK");
            return;
        }

        var todas = await _api.ObtenerMascotasAsync();
        Mascotas = todas.Where(m => m.UsuarioId == usuario.Id).ToList();
    }

    [RelayCommand]
    public async Task RegistrarMascota()
    {
        if (usuario == null)
        {
            var js = Preferences.Get("usuarioJson", string.Empty);
            usuario = JsonSerializer.Deserialize<Usuario>(js);

            if (usuario == null)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo obtener el usuario actual", "OK");
                return;
            }
        }

        int edad = DateTime.Today.Year - FechaNacimiento.Year;
        if (FechaNacimiento > DateTime.Today.AddYears(-edad)) edad--;

        var nueva = new Mascota
        {
            Nombre = Nombre,
            Raza = Raza,
            Especie = Especie,
            FechaNacimiento = FechaNacimiento,
            Edad = edad,
            UsuarioId = usuario.Id
        };

        if (await _api.CrearMascotaAsync(nueva))
        {
            MostrarFormulario = false;
            Nombre = Raza = Especie = string.Empty;
            FechaNacimiento = DateTime.Today;
            await CargarMascotas();
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo registrar la mascota", "OK");
        }
    }
}
