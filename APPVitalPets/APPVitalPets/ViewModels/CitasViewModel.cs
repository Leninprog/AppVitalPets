using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using APPVitalPets.Models;
using APPVitalPets.Services;

namespace APPVitalPets.ViewModels;

public partial class CitasViewModel : ObservableObject
{
    private readonly ApiService _api = new();

    [ObservableProperty] private List<Cita> citas = new();
    [ObservableProperty] private string nombreMascota, nombreDueño, telefono, motivo, veterinario;
    [ObservableProperty] private DateTime fecha = DateTime.Today;
    [ObservableProperty] private bool formularioVisible;
    [ObservableProperty] private bool editando;

    private Cita citaActual;

    public CitasViewModel()
    {
        _ = CargarCitas();
    }

    [RelayCommand]
    public void MostrarFormulario()
    {
        FormularioVisible = true;
        Editando = false;
        NombreMascota = NombreDueño = Telefono = Motivo = Veterinario = string.Empty;
        Fecha = DateTime.Today;
    }

    [RelayCommand]
    public async Task CargarCitas()
    {
        var lista = await _api.ObtenerCitasAsync();
        Citas = lista;
    }

    [RelayCommand]
    public async Task RegistrarCita()
    {
        var nueva = new Cita
        {
            NombreMascota = NombreMascota,
            NombreDueño = NombreDueño,
            TelefonoContacto = Telefono,
            Fecha = Fecha,
            Motivo = Motivo,
            VeterinarioAsignado = Veterinario
        };

        if (await _api.CrearCitaAsync(nueva))
        {
            await Shell.Current.DisplayAlert("Éxito", "Cita registrada", "OK");
            FormularioVisible = false;
            await CargarCitas();
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo registrar", "OK");
        }
    }

    [RelayCommand]
    public void EditarCita(Cita cita)
    {
        citaActual = cita;

        NombreMascota = cita.NombreMascota;
        NombreDueño = cita.NombreDueño;
        Telefono = cita.TelefonoContacto;
        Fecha = cita.Fecha;
        Motivo = cita.Motivo;
        Veterinario = cita.VeterinarioAsignado;

        Editando = true;
        FormularioVisible = true;
    }

    [RelayCommand]
    public async Task ActualizarCita()
    {
        if (citaActual == null) return;

        citaActual.NombreMascota = NombreMascota;
        citaActual.NombreDueño = NombreDueño;
        citaActual.TelefonoContacto = Telefono;
        citaActual.Fecha = Fecha;
        citaActual.Motivo = Motivo;
        citaActual.VeterinarioAsignado = Veterinario;

        if (await _api.ActualizarCitaAsync(citaActual))
        {
            await Shell.Current.DisplayAlert("Éxito", "Cita actualizada", "OK");
            FormularioVisible = false;
            await CargarCitas();
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo actualizar", "OK");
        }
    }

    [RelayCommand]
    public async Task EliminarCita(int id)
    {
        if (await _api.EliminarCitaAsync(id))
        {
            await Shell.Current.DisplayAlert("Eliminado", "Cita eliminada", "OK");
            await CargarCitas();
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo eliminar", "OK");
        }
    }
}
