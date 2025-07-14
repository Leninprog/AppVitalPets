using APPVitalPets.Models;
using APPVitalPets.Services;

namespace APPVitalPets.Views;

public partial class VeterinariosPage : ContentPage
{
    public VeterinariosPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var api = new ApiService();
        VetsList.ItemsSource = await api.ObtenerVeterinariosAsync();
    }

    private async void OnEliminarVet(object sender, EventArgs e)
    {
        var id = (int)(sender as Button).CommandParameter;
        var api = new ApiService();
        if (await api.EliminarVeterinarioAsync(id))
            await DisplayAlert("OK", "Veterinario eliminado", "Cerrar");
        await OnAppearing();
    }

    // Implementa OnMostrarFormularioClicked, OnAgregarVet, OnEditarVet, etc.
}