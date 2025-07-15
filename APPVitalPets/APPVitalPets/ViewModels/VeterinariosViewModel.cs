using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using APPVitalPets.Models;
using APPVitalPets.Services;
using System.Collections.ObjectModel;

namespace APPVitalPets.ViewModels;

public partial class VeterinariosViewModel : ObservableObject
{
    private readonly ApiService _api = new();

    [ObservableProperty] private ObservableCollection<Veterinario> veterinarios = new();
    [ObservableProperty] private string nombre, especialidad, telefono, correo;
    [ObservableProperty] private bool formularioVisible;
    [ObservableProperty] private bool editando;

    private Veterinario vetActual;

    public VeterinariosViewModel()
    {
        _ = CargarVeterinarios();
    }

    [RelayCommand]
    public void MostrarFormulario()
    {
        FormularioVisible = true;
        Editando = false;
        Nombre = Especialidad = Telefono = Correo = string.Empty;
    }

    [RelayCommand]
    public async Task CargarVeterinarios()
    {
        var lista = await _api.ObtenerVeterinariosAsync();
        Veterinarios = new ObservableCollection<Veterinario>(lista);
    }

    [RelayCommand]
    public async Task RegistrarVeterinario()
    {
        var nuevoVet = new Veterinario
        {
            Nombre = Nombre,
            Especialidad = Especialidad,
            Telefono = Telefono,
            Correo = Correo
        };

        if (await _api.CrearVeterinarioAsync(nuevoVet))
        {
            await Shell.Current.DisplayAlert("Éxito", "Veterinario registrado", "OK");
            FormularioVisible = false;
            await CargarVeterinarios();
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo registrar", "OK");
        }
    }

    [RelayCommand]
    public void EditarVeterinario(Veterinario vet)
    {
        vetActual = vet;
        Nombre = vet.Nombre;
        Especialidad = vet.Especialidad;
        Telefono = vet.Telefono;
        Correo = vet.Correo;

        Editando = true;
        FormularioVisible = true;
    }

    [RelayCommand]
    public async Task ActualizarVeterinario()
    {
        if (vetActual == null) return;

        vetActual.Nombre = Nombre;
        vetActual.Especialidad = Especialidad;
        vetActual.Telefono = Telefono;
        vetActual.Correo = Correo;

        if (await _api.ActualizarVeterinarioAsync(vetActual))
        {
            await Shell.Current.DisplayAlert("Éxito", "Veterinario actualizado", "OK");
            FormularioVisible = false;
            await CargarVeterinarios();
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo actualizar", "OK");
        }
    }

    [RelayCommand]
    public async Task EliminarVeterinario(int id)
    {
        if (await _api.EliminarVeterinarioAsync(id))
        {
            await Shell.Current.DisplayAlert("Eliminado", "Veterinario eliminado", "OK");
            await CargarVeterinarios();
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo eliminar", "OK");
        }
    }
}
