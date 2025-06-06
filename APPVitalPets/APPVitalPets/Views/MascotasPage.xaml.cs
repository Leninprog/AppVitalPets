using APPVitalPets.Models;
using APPVitalPets.Services;

namespace APPVitalPets.Views;

public partial class MascotasPage : ContentPage
{
    Usuario usuarioActual;
    public MascotasPage(Usuario user)
    {
        InitializeComponent();
        usuarioActual = user;
        CargarMascotas();
    }

    private async void CargarMascotas()
    {
        var api = new ApiService();
        MascotasList.ItemsSource = await api.ObtenerMascotasAsync();
    }

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        var nueva = new Mascota
        {
            Nombre = "Firulais",
            Raza = "Labrador",
            Edad = "2",
            UsuarioId = usuarioActual.Id
        };
        var api = new ApiService();
        if (await api.CrearMascotaAsync(nueva))
            CargarMascotas();
        else
            await DisplayAlert("Error", "No se pudo crear mascota", "OK");
    }

    private void OnMascotaSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedMascota = e.CurrentSelection.FirstOrDefault() as Mascota;
        if (selectedMascota == null)
            return;

        DisplayAlert("Mascota seleccionada", $"Nombre: {selectedMascota.Nombre}", "OK");

        ((CollectionView)sender).SelectedItem = null;
    }

}